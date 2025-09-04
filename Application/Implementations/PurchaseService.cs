using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Implementations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMarketplaceRepository _marketplaceRepository;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public PurchaseService(
            IPurchaseRepository purchaseRepository,
            IMarketplaceRepository marketplaceRepository,
            IPaymentService paymentService,
            INotificationService notificationService)
        {
            _purchaseRepository = purchaseRepository;
            _marketplaceRepository = marketplaceRepository;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        public async Task<PurchaseResponseDto> ProcessPurchaseAsync(CreatePurchaseDto dto, int factoryId)
        {
            try
            {
                // Validate marketplace item availability
                var marketplaceItem = await _marketplaceRepository.GetByIdAsync(dto.MarketplaceItemId);
                if (marketplaceItem == null)
                {
                    throw new ArgumentException($"Marketplace item {dto.MarketplaceItemId} not found");
                }

                if (!marketplaceItem.IsAvailable)
                {
                    throw new InvalidOperationException($"Marketplace item {dto.MarketplaceItemId} is not available");
                }

                if (marketplaceItem.Quantity < dto.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient quantity available. Requested: {dto.Quantity}, Available: {marketplaceItem.Quantity}");
                }

                // Create purchase record
                var purchase = new Purchase
                {
                    Id = Guid.NewGuid(),
                    MarketplaceItemId = dto.MarketplaceItemId,
                    FactoryId = factoryId,
                    Quantity = dto.Quantity,
                    PricePerUnit = marketplaceItem.PricePerUnit,
                    TotalAmount = dto.Quantity * marketplaceItem.PricePerUnit,
                    PaymentStatus = PaymentStatus.Pending,
                    PurchaseDate = DateTime.UtcNow
                };

                purchase.CalculateTotalAmount();

                var createdPurchase = await _purchaseRepository.CreateAsync(purchase);

                // Process Stripe payment
                var paymentResult = await _paymentService.ProcessPurchasePaymentAsync(createdPurchase.Id, factoryId);

                if (!paymentResult.IsSuccess)
                {
                    // Rollback purchase if payment fails
                    await _purchaseRepository.DeleteAsync(createdPurchase.Id);
                    throw new InvalidOperationException($"Payment failed: {paymentResult.Message}");
                }

                // Update marketplace item availability
                marketplaceItem.Quantity -= dto.Quantity;
                if (marketplaceItem.Quantity <= 0)
                {
                    marketplaceItem.IsAvailable = false;
                }
                marketplaceItem.UpdateTimestamp();
                await _marketplaceRepository.UpdateAsync(marketplaceItem);

                // Send notifications
                await SendPurchaseNotificationsAsync(createdPurchase, marketplaceItem);

                return MapToResponseDto(createdPurchase, paymentResult);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedPurchasesDto> GetByFactoryIdAsync(int factoryId, int page, int pageSize)
        {
            var purchases = await _purchaseRepository.GetByFactoryIdAsync(factoryId, page, pageSize);
            var totalCount = await _purchaseRepository.GetTotalCountByFactoryIdAsync(factoryId);

            return new PaginatedPurchasesDto
            {
                Items = purchases.Select(p => MapToResponseDto(p)),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<PurchaseResponseDto> CreatePurchaseAsync(CreatePurchaseDto dto, int factoryId)
        {
            // This method is now replaced by ProcessPurchaseAsync
            // Keeping for interface compatibility
            return await ProcessPurchaseAsync(dto, factoryId);
        }

        public async Task<PurchaseResponseDto> UpdatePaymentStatusAsync(Guid purchaseId, UpdatePaymentStatusDto dto)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);
            if (purchase == null)
                throw new ArgumentException("Purchase not found");

            purchase.PaymentStatus = dto.Status;
            purchase.UpdateTimestamp();
            await _purchaseRepository.UpdateAsync(purchase);

            return MapToResponseDto(purchase);
        }

        public async Task<PurchaseResponseDto> GetByIdAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                throw new ArgumentException("Purchase not found");

            return MapToResponseDto(purchase);
        }

        private async Task SendPurchaseNotificationsAsync(Purchase purchase, MarketplaceItem marketplaceItem)
        {
            try
            {
                // Notify factory about successful purchase
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = purchase.FactoryId,
                    Type = NotificationType.PurchaseConfirmed,
                    Title = "Purchase Confirmed",
                    Message = $"Your purchase of {purchase.Quantity} {marketplaceItem.Unit} of {marketplaceItem.MaterialType} has been confirmed. Total: ${purchase.TotalAmount}",
                    IsRead = false
                });

                // Notify original seller about item sold
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = marketplaceItem.UserId,
                    Type = NotificationType.ItemSold,
                    Title = "Item Sold",
                    Message = $"Your {marketplaceItem.MaterialType} item has been sold. Quantity: {purchase.Quantity} {marketplaceItem.Unit}, Amount: ${purchase.TotalAmount}",
                    IsRead = false
                });
            }
            catch (Exception)
            {
                // Log notification errors but don't fail the purchase
            }
        }

        private static PurchaseResponseDto MapToResponseDto(Purchase purchase, PaymentResponseDto paymentResult = null)
        {
            return new PurchaseResponseDto
            {
                Id = purchase.Id,
                MarketplaceItemId = purchase.MarketplaceItemId,
                FactoryId = purchase.FactoryId,
                FactoryName = purchase.Factory?.Name,
                Quantity = purchase.Quantity,
                PricePerUnit = purchase.PricePerUnit,
                TotalAmount = purchase.TotalAmount,
                StripePaymentIntentId = purchase.StripePaymentIntentId,
                PaymentStatus = purchase.PaymentStatus,
                PurchaseDate = purchase.PurchaseDate,
                CreatedAt = purchase.CreatedAt,
                UpdatedAt = purchase.UpdatedAt,
                MarketplaceItem = purchase.MarketplaceItem != null ? new MarketplaceItemDto
                {
                    Id = purchase.MarketplaceItem.Id,
                    PickupRequestId = purchase.MarketplaceItem.PickupRequestId,
                    UserId = purchase.MarketplaceItem.UserId,
                    UserName = purchase.MarketplaceItem.User?.Name,
                    MaterialType = purchase.MarketplaceItem.MaterialType,
                    Quantity = purchase.MarketplaceItem.Quantity,
                    Unit = purchase.MarketplaceItem.Unit,
                    PricePerUnit = purchase.MarketplaceItem.PricePerUnit,
                    TotalPrice = purchase.MarketplaceItem.TotalPrice,
                    Description = purchase.MarketplaceItem.Description,
                    ImageUrls = purchase.MarketplaceItem.ImageUrls,
                    IsAvailable = purchase.MarketplaceItem.IsAvailable,
                    PublishedAt = purchase.MarketplaceItem.PublishedAt,
                    CreatedAt = purchase.MarketplaceItem.CreatedAt,
                    UpdatedAt = purchase.MarketplaceItem.UpdatedAt
                } : null,
                PaymentResult = paymentResult
            };
        }
    }
}
