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
        private readonly INotificationService _notificationService;

        public PurchaseService(
            IPurchaseRepository purchaseRepository,
            IMarketplaceRepository marketplaceRepository,
            INotificationService notificationService)
        {
            _purchaseRepository = purchaseRepository;
            _marketplaceRepository = marketplaceRepository;
            _notificationService = notificationService;
        }

        public async Task<PurchaseResponseDto> CreatePurchaseAsync(CreatePurchaseDto dto, int factoryId)
        {
            // Check if marketplace item exists and is available
            var marketplaceItem = await _marketplaceRepository.GetByIdAsync(dto.MarketplaceItemId);
            if (marketplaceItem == null)
                throw new ArgumentException("Marketplace item not found");

            if (!marketplaceItem.IsAvailable)
                throw new InvalidOperationException("Marketplace item is not available for purchase");

            // Check if item is already purchased
            var existingPurchase = await _purchaseRepository.GetByMarketplaceItemIdAsync(dto.MarketplaceItemId);
            if (existingPurchase != null)
                throw new InvalidOperationException("Item has already been purchased");

            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                MarketplaceItemId = dto.MarketplaceItemId,
                FactoryId = factoryId,
                Quantity = dto.Quantity,
                PricePerUnit = dto.PricePerUnit,
                StripePaymentIntentId = dto.StripePaymentIntentId,
                PaymentStatus = PaymentStatus.Pending,
                PurchaseDate = DateTime.UtcNow
            };

            var createdPurchase = await _purchaseRepository.CreateAsync(purchase);

            // Mark marketplace item as unavailable
            await _marketplaceRepository.UpdateAvailabilityAsync(dto.MarketplaceItemId, false);

            // Create notification for the seller
            await _notificationService.CreateNotificationAsync(
                marketplaceItem.UserId,
                "Item Sold",
                $"Your {marketplaceItem.MaterialType} item has been purchased by a factory.",
                NotificationType.ItemSold);

            return MapToResponseDto(createdPurchase);
        }

        public async Task<PaginatedPurchasesDto> GetByFactoryIdAsync(int factoryId, int page, int pageSize)
        {
            var purchases = await _purchaseRepository.GetByFactoryIdAsync(factoryId, page, pageSize);
            var totalCount = await _purchaseRepository.GetTotalCountByFactoryIdAsync(factoryId);

            return new PaginatedPurchasesDto
            {
                Items = purchases.Select(MapToResponseDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<PurchaseResponseDto> GetByIdAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                throw new ArgumentException("Purchase not found");

            return MapToResponseDto(purchase);
        }

        public async Task<PurchaseResponseDto> UpdatePaymentStatusAsync(Guid purchaseId, UpdatePaymentStatusDto dto)
        {
            var purchase = await _purchaseRepository.UpdatePaymentStatusAsync(purchaseId, dto.Status);
            if (purchase == null)
                throw new ArgumentException("Purchase not found");

            // Create notification based on payment status
            var marketplaceItem = await _marketplaceRepository.GetByIdAsync(purchase.MarketplaceItemId);
            if (marketplaceItem != null)
            {
                string notificationTitle = dto.Status switch
                {
                    PaymentStatus.Completed => "Payment Completed",
                    PaymentStatus.Failed => "Payment Failed",
                    PaymentStatus.Refunded => "Payment Refunded",
                    _ => "Payment Status Updated"
                };

                string notificationMessage = dto.Status switch
                {
                    PaymentStatus.Completed => $"Payment for your {marketplaceItem.MaterialType} item has been completed successfully.",
                    PaymentStatus.Failed => $"Payment for your {marketplaceItem.MaterialType} item has failed. Please contact support.",
                    PaymentStatus.Refunded => $"Payment for your {marketplaceItem.MaterialType} item has been refunded.",
                    _ => $"Payment status for your {marketplaceItem.MaterialType} item has been updated to {dto.Status}."
                };

                await _notificationService.CreateNotificationAsync(
                    marketplaceItem.UserId,
                    notificationTitle,
                    notificationMessage,
                    NotificationType.PaymentReceived);
            }

            return MapToResponseDto(purchase);
        }

        private static PurchaseResponseDto MapToResponseDto(Purchase purchase)
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
                } : null
            };
        }
    }
}
