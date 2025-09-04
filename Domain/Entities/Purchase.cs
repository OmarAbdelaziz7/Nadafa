using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Purchase
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MarketplaceItemId { get; set; }

        [Required]
        public int FactoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerUnit { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual MarketplaceItem MarketplaceItem { get; set; }
        public virtual Factory Factory { get; set; }

        // Helper method to update timestamp
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper method to calculate and set total amount
        public void CalculateTotalAmount()
        {
            TotalAmount = Quantity * PricePerUnit;
        }
    }
}
