using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PickupRequest
    {
        public Guid Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public MaterialType MaterialType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        public Unit Unit { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ProposedPricePerUnit { get; set; }

        public decimal TotalEstimatedPrice => Quantity * ProposedPricePerUnit;

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        [Required]
        public PickupStatus Status { get; set; } = PickupStatus.Pending;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedDate { get; set; }

        public DateTime? PickupDate { get; set; }

        public int? AdminId { get; set; }

        [StringLength(1000)]
        public string AdminNotes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual User Admin { get; set; }
        public virtual MarketplaceItem MarketplaceItem { get; set; }

        // Helper method to update timestamp
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
