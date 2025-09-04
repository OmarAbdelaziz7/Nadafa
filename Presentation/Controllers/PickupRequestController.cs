using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PickupRequestController : ControllerBase
    {
        private readonly IPickupRequestService _pickupRequestService;

        public PickupRequestController(IPickupRequestService pickupRequestService)
        {
            _pickupRequestService = pickupRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<PickupRequestResponseDto>> CreateRequest([FromBody] CreatePickupRequestDto dto)
        {
            try
            {
                // Get user ID from JWT token (you'll need to implement this based on your auth setup)
                var userId = GetUserIdFromToken();
                var result = await _pickupRequestService.CreateRequestAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<PaginatedPickupRequestsDto>> GetUserRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _pickupRequestService.GetUserRequestsAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
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
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PickupRequestResponseDto>> GetById(Guid id)
        {
            try
            {
                var result = await _pickupRequestService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PickupRequestResponseDto>> ApproveRequest(
            Guid id,
            [FromBody] ApprovePickupRequestDto dto)
        {
            try
            {
                var adminId = GetUserIdFromToken();
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
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PickupRequestResponseDto>> RejectRequest(
            Guid id,
            [FromBody] RejectPickupRequestDto dto)
        {
            try
            {
                var adminId = GetUserIdFromToken();
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
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        private int GetUserIdFromToken()
        {
            // This is a placeholder - you need to implement this based on your JWT token structure
            // The user ID should be extracted from the JWT token claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}
