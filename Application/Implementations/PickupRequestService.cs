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
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IMarketplaceRepository _marketplaceRepository;

        public PickupRequestService(
            IPickupRequestRepository pickupRequestRepository,
            IPaymentService paymentService,
            INotificationService notificationService,
            IMarketplaceRepository marketplaceRepository)
        {
            _pickupRequestRepository = pickupRequestRepository;
            _paymentService = paymentService;
            _notificationService = notificationService;
            _marketplaceRepository = marketplaceRepository;
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
            try
            {
                var request = await _pickupRequestRepository.GetByIdAsync(requestId);
                if (request == null)
                    throw new ArgumentException("Pickup request not found");

                if (request.Status != PickupStatus.Pending)
                    throw new InvalidOperationException("Request is not in pending status");

                // Approve the pickup request
                request.Status = PickupStatus.Approved;
                request.AdminId = adminId;
                request.AdminNotes = dto.Notes;
                request.ApprovedDate = DateTime.UtcNow;

                var updatedRequest = await _pickupRequestRepository.UpdateAsync(request);

                // Process payment to user automatically
                var paymentResult = await _paymentService.ProcessPickupPaymentAsync(requestId, request.TotalEstimatedPrice);

                if (!paymentResult.IsSuccess)
                {
                    // Rollback approval if payment fails
                    request.Status = PickupStatus.Pending;
                    request.AdminId = null;
                    request.AdminNotes = null;
                    request.ApprovedDate = null;
                    await _pickupRequestRepository.UpdateAsync(request);
                    throw new InvalidOperationException($"Payment failed: {paymentResult.Message}");
                }

                // Create marketplace item
                var marketplaceItem = new MarketplaceItem
                {
                    Id = Guid.NewGuid(),
                    PickupRequestId = request.Id,
                    UserId = request.UserId,
                    MaterialType = request.MaterialType,
                    Quantity = request.Quantity,
                    Unit = request.Unit,
                    PricePerUnit = request.ProposedPricePerUnit,
                    Description = request.Description,
                    ImageUrls = request.ImageUrls,
                    IsAvailable = true,
                    PublishedAt = DateTime.UtcNow
                };

                await _marketplaceRepository.CreateAsync(marketplaceItem);

                // Update pickup request status to published
                request.Status = PickupStatus.Published;
                await _pickupRequestRepository.UpdateAsync(request);

                // Send notifications
                await SendApprovalNotificationsAsync(updatedRequest, paymentResult);

                return MapToResponseDto(updatedRequest);
            }
            catch (Exception)
            {
                throw;
            }
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

        private async Task SendApprovalNotificationsAsync(PickupRequest request, PaymentResponseDto paymentResult)
        {
            try
            {
                // Notify user about pickup approval and payment
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = request.UserId,
                    Type = NotificationType.PickupApproved,
                    Title = "Pickup Request Approved",
                    Message = $"Your pickup request for {request.Quantity} {request.Unit} of {request.MaterialType} has been approved. Payment of ${request.TotalEstimatedPrice} has been processed.",
                    IsRead = false
                });

                // Notify user about marketplace item creation
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = request.UserId,
                    Type = NotificationType.PaymentReceived,
                    Title = "Payment Received",
                    Message = $"You have received ${request.TotalEstimatedPrice} for your {request.MaterialType} pickup. Your item is now available in the marketplace.",
                    IsRead = false
                });
            }
            catch (Exception)
            {
                // Log notification errors but don't fail the approval process
            }
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
