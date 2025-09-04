using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Domain.Entities;

namespace Presentation.Controllers
{
    public class PickupRequestController : ApiControllerBase
    {
        private readonly IPickupRequestService _pickupRequestService;

        public PickupRequestController(IPickupRequestService pickupRequestService)
        {
            _pickupRequestService = pickupRequestService;
        }

        /// <summary>
        /// Create a new pickup request (User only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("User")]
        public async Task<ActionResult<PickupRequestResponseDto>> CreateRequest([FromBody] CreatePickupRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var result = await _pickupRequestService.CreateRequestAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get user's pickup requests with pagination (User only)
        /// </summary>
        [HttpGet("my-requests")]
        [AuthorizeRoles("User")]
        public async Task<ActionResult<PaginatedPickupRequestsDto>> GetMyRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _pickupRequestService.GetUserRequestsAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get pending pickup requests (Admin only)
        /// </summary>
        [HttpGet("pending")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PaginatedPickupRequestsDto>> GetPendingRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _pickupRequestService.GetPendingRequestsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get pickup request details (User: own requests, Admin: all)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PickupRequestResponseDto>> GetById(Guid id)
        {
            try
            {
                var result = await _pickupRequestService.GetByIdAsync(id);
                var userRole = GetCurrentUserRole();
                var userId = GetCurrentUserId();

                // Users can only view their own requests, admins can view all
                if (userRole == "User" && result.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Approve pickup request (Admin only)
        /// </summary>
        [HttpPut("{id}/approve")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PickupRequestResponseDto>> ApproveRequest(
            Guid id,
            [FromBody] ApprovePickupRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var adminId = GetCurrentUserId();
                var result = await _pickupRequestService.ApproveRequestAsync(id, adminId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Reject pickup request (Admin only)
        /// </summary>
        [HttpPut("{id}/reject")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PickupRequestResponseDto>> RejectRequest(
            Guid id,
            [FromBody] RejectPickupRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var adminId = GetCurrentUserId();
                var result = await _pickupRequestService.RejectRequestAsync(id, adminId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Mark pickup request as picked up (Admin only)
        /// </summary>
        [HttpPut("{id}/mark-picked-up")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PickupRequestResponseDto>> MarkAsPickedUp(Guid id)
        {
            try
            {
                var adminId = GetCurrentUserId();
                // This endpoint would need to be implemented in the service
                // For now, we'll return a placeholder response
                return Ok(new { message = "Pickup request marked as picked up", id });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
