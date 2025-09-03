using System;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts
{
    public interface IPickupRequestService
    {
        Task<PickupRequestResponseDto> CreateRequestAsync(CreatePickupRequestDto dto, int userId);
        Task<PaginatedPickupRequestsDto> GetUserRequestsAsync(int userId, int page, int pageSize);
        Task<PaginatedPickupRequestsDto> GetPendingRequestsAsync(int page, int pageSize);
        Task<PickupRequestResponseDto> ApproveRequestAsync(Guid requestId, int adminId, ApprovePickupRequestDto dto);
        Task<PickupRequestResponseDto> RejectRequestAsync(Guid requestId, int adminId, RejectPickupRequestDto dto);
        Task<PickupRequestResponseDto> GetByIdAsync(Guid id);
    }
}
