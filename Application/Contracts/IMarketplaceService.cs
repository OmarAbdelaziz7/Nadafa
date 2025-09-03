using System;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IMarketplaceService
    {
        Task<PaginatedMarketplaceItemsDto> GetAvailableItemsAsync(MaterialType? filter, int page, int pageSize);
        Task<PaginatedMarketplaceItemsDto> SearchItemsAsync(string searchTerm, int page, int pageSize);
        Task<MarketplaceItemDto> GetItemDetailsAsync(Guid itemId);
        Task<MarketplaceItemDto> CreateMarketplaceItemAsync(Guid pickupRequestId);
    }
}
