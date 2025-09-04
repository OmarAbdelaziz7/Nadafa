using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs
{
    public class CreatePurchaseDto
    {
        [Required]
        public Guid MarketplaceItemId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than 0")]
        public decimal PricePerUnit { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Stripe payment intent ID cannot exceed 100 characters")]
        public string StripePaymentIntentId { get; set; }
    }

    public class PurchaseResponseDto
    {
        public Guid Id { get; set; }
        public Guid MarketplaceItemId { get; set; }
        public int FactoryId { get; set; }
        public string FactoryName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalAmount { get; set; }
        public string StripePaymentIntentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MarketplaceItemDto MarketplaceItem { get; set; }
        public PaymentResponseDto PaymentResult { get; set; }
    }

    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public Guid MarketplaceItemId { get; set; }
        public int FactoryId { get; set; }
        public string FactoryName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalAmount { get; set; }
        public string StripePaymentIntentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PaginatedPurchasesDto
    {
        public IEnumerable<PurchaseResponseDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        [Required]
        public PaymentStatus Status { get; set; }
    }
}
