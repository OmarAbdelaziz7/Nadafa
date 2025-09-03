using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs
{
    public class CreatePickupRequestDto
    {
        [Required]
        public MaterialType MaterialType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required]
        public Unit Unit { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than 0")]
        public decimal ProposedPricePerUnit { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class PickupRequestResponseDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public MaterialType MaterialType { get; set; }
        public decimal Quantity { get; set; }
        public Unit Unit { get; set; }
        public decimal ProposedPricePerUnit { get; set; }
        public decimal TotalEstimatedPrice { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; }
        public PickupStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public int? AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MarketplaceItemDto MarketplaceItem { get; set; }
    }

    public class PaginatedPickupRequestsDto
    {
        public IEnumerable<PickupRequestResponseDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class ApprovePickupRequestDto
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string Notes { get; set; }
    }

    public class RejectPickupRequestDto
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string Notes { get; set; }
    }
}
