using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Implementations
{
    public class PickupRequestService : IPickupRequestService
    {
        private readonly IPickupRequestRepository _pickupRequestRepository;

        public PickupRequestService(IPickupRequestRepository pickupRequestRepository)
        {
            _pickupRequestRepository = pickupRequestRepository;
        }

        public async Task<PickupRequestResponseDto> CreateRequestAsync(CreatePickupRequestDto dto, int userId)
        {
            var pickupRequest = new PickupRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MaterialType = dto.MaterialType,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                ProposedPricePerUnit = dto.ProposedPricePerUnit,
                Description = dto.Description,
                ImageUrls = dto.ImageUrls ?? new System.Collections.Generic.List<string>(),
                Status = PickupStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            var createdRequest = await _pickupRequestRepository.CreateAsync(pickupRequest);
            return MapToResponseDto(createdRequest);
        }

        public async Task<PaginatedPickupRequestsDto> GetUserRequestsAsync(int userId, int page, int pageSize)
        {
            var requests = await _pickupRequestRepository.GetByUserIdAsync(userId, page, pageSize);
            var totalCount = await _pickupRequestRepository.GetTotalCountByUserIdAsync(userId);

            return new PaginatedPickupRequestsDto
            {
                Items = requests.Select(MapToResponseDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<PaginatedPickupRequestsDto> GetPendingRequestsAsync(int page, int pageSize)
        {
            var requests = await _pickupRequestRepository.GetPendingRequestsAsync(page, pageSize);
            var totalCount = await _pickupRequestRepository.GetTotalPendingCountAsync();

            return new PaginatedPickupRequestsDto
            {
                Items = requests.Select(MapToResponseDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }

        public async Task<PickupRequestResponseDto> ApproveRequestAsync(Guid requestId, int adminId, ApprovePickupRequestDto dto)
        {
            var request = await _pickupRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new ArgumentException("Pickup request not found");

            if (request.Status != PickupStatus.Pending)
                throw new InvalidOperationException("Request is not in pending status");

            request.Status = PickupStatus.Approved;
            request.AdminId = adminId;
            request.AdminNotes = dto.Notes;
            request.ApprovedDate = DateTime.UtcNow;

            var updatedRequest = await _pickupRequestRepository.UpdateAsync(request);

            return MapToResponseDto(updatedRequest);
        }

        public async Task<PickupRequestResponseDto> RejectRequestAsync(Guid requestId, int adminId, RejectPickupRequestDto dto)
        {
            var request = await _pickupRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new ArgumentException("Pickup request not found");

            if (request.Status != PickupStatus.Pending)
                throw new InvalidOperationException("Request is not in pending status");

            request.Status = PickupStatus.Rejected;
            request.AdminId = adminId;
            request.AdminNotes = dto.Notes;

            var updatedRequest = await _pickupRequestRepository.UpdateAsync(request);

            return MapToResponseDto(updatedRequest);
        }

        public async Task<PickupRequestResponseDto> GetByIdAsync(Guid id)
        {
            var request = await _pickupRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new ArgumentException("Pickup request not found");

            return MapToResponseDto(request);
        }

        private static PickupRequestResponseDto MapToResponseDto(PickupRequest request)
        {
            return new PickupRequestResponseDto
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = request.User?.Name,
                MaterialType = request.MaterialType,
                Quantity = request.Quantity,
                Unit = request.Unit,
                ProposedPricePerUnit = request.ProposedPricePerUnit,
                TotalEstimatedPrice = request.TotalEstimatedPrice,
                Description = request.Description,
                ImageUrls = request.ImageUrls,
                Status = request.Status,
                RequestDate = request.RequestDate,
                ApprovedDate = request.ApprovedDate,
                PickupDate = request.PickupDate,
                AdminId = request.AdminId,
                AdminName = request.Admin?.Name,
                AdminNotes = request.AdminNotes,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                MarketplaceItem = request.MarketplaceItem != null ? new MarketplaceItemDto
                {
                    Id = request.MarketplaceItem.Id,
                    PickupRequestId = request.MarketplaceItem.PickupRequestId,
                    UserId = request.MarketplaceItem.UserId,
                    UserName = request.MarketplaceItem.User?.Name,
                    MaterialType = request.MarketplaceItem.MaterialType,
                    Quantity = request.MarketplaceItem.Quantity,
                    Unit = request.MarketplaceItem.Unit,
                    PricePerUnit = request.MarketplaceItem.PricePerUnit,
                    TotalPrice = request.MarketplaceItem.TotalPrice,
                    Description = request.MarketplaceItem.Description,
                    ImageUrls = request.MarketplaceItem.ImageUrls,
                    IsAvailable = request.MarketplaceItem.IsAvailable,
                    PublishedAt = request.MarketplaceItem.PublishedAt,
                    CreatedAt = request.MarketplaceItem.CreatedAt,
                    UpdatedAt = request.MarketplaceItem.UpdatedAt
                } : null
            };
        }
    }
}
