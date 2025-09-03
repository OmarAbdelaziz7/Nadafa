using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IPickupRequestRepository
    {
        Task<PickupRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<PickupRequest>> GetByUserIdAsync(int userId, int page, int pageSize);
        Task<IEnumerable<PickupRequest>> GetPendingRequestsAsync(int page, int pageSize);
        Task<PickupRequest> CreateAsync(PickupRequest request);
        Task<PickupRequest> UpdateAsync(PickupRequest request);
        Task<IEnumerable<PickupRequest>> GetRequestsWithItemsAsync();
        Task<int> GetTotalCountByUserIdAsync(int userId);
        Task<int> GetTotalPendingCountAsync();
    }
}
