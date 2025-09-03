using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Implementations
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository _marketplaceRepository;
        private readonly IPickupRequestRepository _pickupRequestRepository;

        public MarketplaceService(
            IMarketplaceRepository marketplaceRepository,
            IPickupRequestRepository pickupRequestRepository)
        {
            _marketplaceRepository = marketplaceRepository;
            _pickupRequestRepository = pickupRequestRepository;
        }

        public async Task<PaginatedMarketplaceItemsDto> GetAvailableItemsAsync(MaterialType? filter, int page, int pageSize)
        {
            var items = await _marketplaceRepository.GetAvailableItemsAsync(filter, page, pageSize);
            var totalCount = await _marketplaceRepository.GetTotalAvailableCountAsync(filter);

            return new PaginatedMarketplaceItemsDto
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<PaginatedMarketplaceItemsDto> SearchItemsAsync(string searchTerm, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty");

            var items = await _marketplaceRepository.SearchItemsAsync(searchTerm, page, pageSize);
            var totalCount = await _marketplaceRepository.GetTotalSearchCountAsync(searchTerm);

            return new PaginatedMarketplaceItemsDto
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<MarketplaceItemDto> GetItemDetailsAsync(Guid itemId)
        {
            var item = await _marketplaceRepository.GetByIdAsync(itemId);
            if (item == null)
                throw new ArgumentException("Marketplace item not found");

            return MapToDto(item);
        }

        public async Task<MarketplaceItemDto> CreateMarketplaceItemAsync(Guid pickupRequestId)
        {
            var pickupRequest = await _pickupRequestRepository.GetByIdAsync(pickupRequestId);
            if (pickupRequest == null)
                throw new ArgumentException("Pickup request not found");

            if (pickupRequest.Status != PickupStatus.Approved)
                throw new InvalidOperationException("Pickup request must be approved before creating marketplace item");

            var marketplaceItem = new MarketplaceItem
            {
                Id = Guid.NewGuid(),
                PickupRequestId = pickupRequestId,
                UserId = pickupRequest.UserId,
                MaterialType = pickupRequest.MaterialType,
                Quantity = pickupRequest.Quantity,
                Unit = pickupRequest.Unit,
                PricePerUnit = pickupRequest.ProposedPricePerUnit,
                Description = pickupRequest.Description,
                ImageUrls = pickupRequest.ImageUrls,
                IsAvailable = true,
                PublishedAt = DateTime.UtcNow
            };

            var createdItem = await _marketplaceRepository.CreateAsync(marketplaceItem);

            // Update pickup request status
            pickupRequest.Status = PickupStatus.Published;
            await _pickupRequestRepository.UpdateAsync(pickupRequest);

            return MapToDto(createdItem);
        }

        private static MarketplaceItemDto MapToDto(MarketplaceItem item)
        {
            return new MarketplaceItemDto
            {
                Id = item.Id,
                PickupRequestId = item.PickupRequestId,
                UserId = item.UserId,
                UserName = item.User?.Name,
                MaterialType = item.MaterialType,
                Quantity = item.Quantity,
                Unit = item.Unit,
                PricePerUnit = item.PricePerUnit,
                TotalPrice = item.TotalPrice,
                Description = item.Description,
                ImageUrls = item.ImageUrls,
                IsAvailable = item.IsAvailable,
                PublishedAt = item.PublishedAt,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Purchase = item.Purchase != null ? new PurchaseDto
                {
                    Id = item.Purchase.Id,
                    MarketplaceItemId = item.Purchase.MarketplaceItemId,
                    FactoryId = item.Purchase.FactoryId,
                    FactoryName = item.Purchase.Factory?.Name,
                    Quantity = item.Purchase.Quantity,
                    PricePerUnit = item.Purchase.PricePerUnit,
                    TotalAmount = item.Purchase.TotalAmount,
                    StripePaymentIntentId = item.Purchase.StripePaymentIntentId,
                    PaymentStatus = item.Purchase.PaymentStatus,
                    PurchaseDate = item.Purchase.PurchaseDate,
                    CreatedAt = item.Purchase.CreatedAt,
                    UpdatedAt = item.Purchase.UpdatedAt
                } : null
            };
        }
    }
}
