using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IMarketplaceRepository
    {
        Task<IEnumerable<MarketplaceItem>> GetAvailableItemsAsync(MaterialType? filter, int page, int pageSize);
        Task<MarketplaceItem> GetByIdAsync(Guid id);
        Task<MarketplaceItem> CreateAsync(MarketplaceItem item);
        Task<MarketplaceItem> UpdateAvailabilityAsync(Guid id, bool isAvailable);
        Task<IEnumerable<MarketplaceItem>> SearchItemsAsync(string searchTerm, int page, int pageSize);
        Task<int> GetTotalAvailableCountAsync(MaterialType? filter);
        Task<int> GetTotalSearchCountAsync(string searchTerm);
    }
}
