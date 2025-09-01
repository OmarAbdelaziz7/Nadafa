using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MarketplaceItem
    {
        public int Id { get; set; }
        
        public int PickupRequestId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MaterialType { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal Weight { get; set; }
        
        [StringLength(20)]
        public string WeightUnit { get; set; } = "kg";
        
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        
        public MarketplaceItemStatus Status { get; set; } = MarketplaceItemStatus.Available;
        
        public int? PurchasedByFactoryId { get; set; }
        
        public DateTime? PurchaseDate { get; set; }
        
        public decimal? PurchasePrice { get; set; }
        
        [StringLength(100)]
        public string StripePaymentIntentId { get; set; }
        
        // Navigation properties
        public virtual PickupRequest PickupRequest { get; set; }
        public virtual Factory PurchasedByFactory { get; set; }
    }
    
    public enum MarketplaceItemStatus
    {
        Available = 1,
        Reserved = 2,
        Sold = 3,
        Removed = 4
    }
}
