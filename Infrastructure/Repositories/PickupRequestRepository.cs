using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PickupRequestRepository : IPickupRequestRepository
    {
        private readonly NadafaDbContext _context;

        public PickupRequestRepository(NadafaDbContext context)
        {
            _context = context;
        }

        public async Task<PickupRequest> GetByIdAsync(Guid id)
        {
            return await _context.PickupRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Admin)
                .Include(pr => pr.MarketplaceItem)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<IEnumerable<PickupRequest>> GetByUserIdAsync(int userId, int page, int pageSize)
        {
            return await _context.PickupRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Admin)
                .Include(pr => pr.MarketplaceItem)
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<PickupRequest>> GetPendingRequestsAsync(int page, int pageSize)
        {
            return await _context.PickupRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Admin)
                .Include(pr => pr.MarketplaceItem)
                .Where(pr => pr.Status == PickupStatus.Pending)
                .OrderBy(pr => pr.RequestDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PickupRequest> CreateAsync(PickupRequest request)
        {
            request.UpdateTimestamp();
            _context.PickupRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<PickupRequest> UpdateAsync(PickupRequest request)
        {
            request.UpdateTimestamp();
            _context.PickupRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<PickupRequest>> GetRequestsWithItemsAsync()
        {
            return await _context.PickupRequests
                .Include(pr => pr.User)
                .Include(pr => pr.Admin)
                .Include(pr => pr.MarketplaceItem)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _context.PickupRequests
                .Where(pr => pr.UserId == userId)
                .CountAsync();
        }

        public async Task<int> GetTotalPendingCountAsync()
        {
            return await _context.PickupRequests
                .Where(pr => pr.Status == PickupStatus.Pending)
                .CountAsync();
        }
    }
}
