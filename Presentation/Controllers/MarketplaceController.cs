using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Domain.Entities;

namespace Presentation.Controllers
{
    public class MarketplaceController : ApiControllerBase
    {
        private readonly IMarketplaceService _marketplaceService;
        private readonly IPurchaseService _purchaseService;

        public MarketplaceController(
            IMarketplaceService marketplaceService,
            IPurchaseService purchaseService)
        {
            _marketplaceService = marketplaceService;
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Get available marketplace items with filters (All authenticated users)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedMarketplaceItemsDto>> GetAvailableItems(
            [FromQuery] MaterialType? materialType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _marketplaceService.GetAvailableItemsAsync(materialType, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get marketplace item details (All authenticated users)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketplaceItemDto>> GetItemDetails(Guid id)
        {
            try
            {
                var result = await _marketplaceService.GetItemDetailsAsync(id);
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
        /// Search marketplace items (All authenticated users)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedMarketplaceItemsDto>> SearchItems(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { error = "Search term is required" });
                }

                var result = await _marketplaceService.SearchItemsAsync(searchTerm, page, pageSize);
                return Ok(result);
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
        /// Purchase marketplace item (Factory only)
        /// </summary>
        [HttpPost("{id}/purchase")]
        [AuthorizeRoles("Factory")]
        public async Task<ActionResult<PurchaseResponseDto>> PurchaseItem(
            Guid id,
            [FromBody] CreatePurchaseDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ensure the marketplace item ID matches the route parameter
                if (dto.MarketplaceItemId != id)
                {
                    return BadRequest(new { error = "Marketplace item ID mismatch" });
                }

                var factoryId = GetCurrentUserId();
                var result = await _purchaseService.CreatePurchaseAsync(dto, factoryId);
                return CreatedAtAction(nameof(GetItemDetails), new { id = result.MarketplaceItemId }, result);
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
        /// Get user's sold items (User only)
        /// </summary>
        [HttpGet("my-sales")]
        [AuthorizeRoles("User")]
        public async Task<ActionResult<PaginatedMarketplaceItemsDto>> GetMySales(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                // This would need to be implemented in the marketplace service
                // For now, we'll return a placeholder response
                return Ok(new PaginatedMarketplaceItemsDto
                {
                    Items = new System.Collections.Generic.List<MarketplaceItemDto>(),
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
        /// Get marketplace statistics (Admin only)
        /// </summary>
        [HttpGet("stats")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<object>> GetMarketplaceStats()
        {
            try
            {
                // This would need to be implemented in the marketplace service
                // For now, we'll return a placeholder response
                return Ok(new
                {
                    TotalItems = 0,
                    AvailableItems = 0,
                    SoldItems = 0,
                    TotalRevenue = 0.0m,
                    AveragePrice = 0.0m
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
