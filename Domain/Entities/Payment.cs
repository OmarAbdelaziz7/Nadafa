using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }

        [Required]
        [StringLength(100)]
        public string StripeCustomerId { get; set; }

        public int? UserId { get; set; }

        public int? FactoryId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public PaymentType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string StripeResponse { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Factory Factory { get; set; }
    }



    public enum PaymentType
    {
        UserPickup = 1,
        FactoryPurchase = 2,
        Subscription = 3
    }
}
