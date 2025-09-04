using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;

namespace Presentation.Controllers
{
    public class PurchaseController : ApiControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Get factory's purchases (Factory only)
        /// </summary>
        [HttpGet("my-purchases")]
        [AuthorizeRoles("Factory")]
        public async Task<ActionResult<PaginatedPurchasesDto>> GetMyPurchases(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var factoryId = GetCurrentUserId();
                var result = await _purchaseService.GetByFactoryIdAsync(factoryId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get purchase details (Factory: own purchases, Admin: all)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseResponseDto>> GetPurchaseDetails(Guid id)
        {
            try
            {
                var result = await _purchaseService.GetByIdAsync(id);
                var userRole = GetCurrentUserRole();
                var userId = GetCurrentUserId();

                // Factories can only view their own purchases, admins can view all
                if (userRole == "Factory" && result.FactoryId != userId)
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
        /// Confirm payment for purchase (Factory only)
        /// </summary>
        [HttpPost("{id}/confirm-payment")]
        [AuthorizeRoles("Factory")]
        public async Task<ActionResult<PurchaseResponseDto>> ConfirmPayment(
            Guid id,
            [FromBody] UpdatePaymentStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var factoryId = GetCurrentUserId();

                // Verify the purchase belongs to the factory
                var purchase = await _purchaseService.GetByIdAsync(id);
                if (purchase.FactoryId != factoryId)
                {
                    return Forbid();
                }

                var result = await _purchaseService.UpdatePaymentStatusAsync(id, dto);
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
        /// Get all purchases (Admin only)
        /// </summary>
        [HttpGet("all")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PaginatedPurchasesDto>> GetAllPurchases(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? factoryId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                // This would need to be implemented in the purchase service
                // For now, we'll return a placeholder response
                return Ok(new PaginatedPurchasesDto
                {
                    Items = new System.Collections.Generic.List<PurchaseResponseDto>(),
                    TotalCount = 0,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = 0,
                    HasNextPage = false,
                    HasPreviousPage = false
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get purchase statistics (Admin only)
        /// </summary>
        [HttpGet("stats")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<object>> GetPurchaseStats()
        {
            try
            {
                // This would need to be implemented in the purchase service
                // For now, we'll return a placeholder response
                return Ok(new
                {
                    TotalPurchases = 0,
                    PendingPayments = 0,
                    CompletedPayments = 0,
                    TotalRevenue = 0.0m,
                    AveragePurchaseValue = 0.0m
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
