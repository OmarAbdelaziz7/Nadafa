using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly NadafaDbContext _context;

        public MarketplaceRepository(NadafaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarketplaceItem>> GetAvailableItemsAsync(MaterialType? filter, int page, int pageSize)
        {
            var query = _context.MarketplaceItems
                .Include(mi => mi.User)
                .Include(mi => mi.PickupRequest)
                .Include(mi => mi.Purchase)
                .Where(mi => mi.IsAvailable);

            if (filter.HasValue)
            {
                query = query.Where(mi => mi.MaterialType == filter.Value);
            }

            return await query
                .OrderByDescending(mi => mi.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<MarketplaceItem> GetByIdAsync(Guid id)
        {
            return await _context.MarketplaceItems
                .Include(mi => mi.User)
                .Include(mi => mi.PickupRequest)
                .Include(mi => mi.Purchase)
                .FirstOrDefaultAsync(mi => mi.Id == id);
        }

        public async Task<MarketplaceItem> CreateAsync(MarketplaceItem item)
        {
            item.UpdateTimestamp();
            _context.MarketplaceItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<MarketplaceItem> UpdateAvailabilityAsync(Guid id, bool isAvailable)
        {
            var item = await _context.MarketplaceItems.FindAsync(id);
            if (item != null)
            {
                item.IsAvailable = isAvailable;
                item.UpdateTimestamp();
                await _context.SaveChangesAsync();
            }
            return item;
        }

        public async Task<IEnumerable<MarketplaceItem>> SearchItemsAsync(string searchTerm, int page, int pageSize)
        {
            return await _context.MarketplaceItems
                .Include(mi => mi.User)
                .Include(mi => mi.PickupRequest)
                .Include(mi => mi.Purchase)
                .Where(mi => mi.IsAvailable &&
                            (mi.Description.Contains(searchTerm) ||
                             mi.MaterialType.ToString().Contains(searchTerm)))
                .OrderByDescending(mi => mi.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalAvailableCountAsync(MaterialType? filter)
        {
            var query = _context.MarketplaceItems.Where(mi => mi.IsAvailable);

            if (filter.HasValue)
            {
                query = query.Where(mi => mi.MaterialType == filter.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalSearchCountAsync(string searchTerm)
        {
            return await _context.MarketplaceItems
                .Where(mi => mi.IsAvailable &&
                            (mi.Description.Contains(searchTerm) ||
                             mi.MaterialType.ToString().Contains(searchTerm)))
                .CountAsync();
        }
    }
}
