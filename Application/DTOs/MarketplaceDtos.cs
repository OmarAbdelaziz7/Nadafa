using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Application.DTOs
{
    public class MarketplaceItemDto
    {
        public Guid Id { get; set; }
        public Guid PickupRequestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public MaterialType MaterialType { get; set; }
        public decimal Quantity { get; set; }
        public Unit Unit { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PurchaseDto Purchase { get; set; }
    }

    public class PaginatedMarketplaceItemsDto
    {
        public IEnumerable<MarketplaceItemDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class MarketplaceSearchDto
    {
        public string SearchTerm { get; set; }
        public MaterialType? MaterialTypeFilter { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
