using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PickupRequest
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
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
        
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? PreferredPickupDate { get; set; }
        
        public PickupStatus Status { get; set; } = PickupStatus.Pending;
        
        public DateTime? ApprovedDate { get; set; }
        
        public int? ApprovedByAdminId { get; set; }
        
        public DateTime? PickupDate { get; set; }
        
        [StringLength(1000)]
        public string AdminNotes { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
        public virtual User ApprovedByAdmin { get; set; }
        public virtual MarketplaceItem MarketplaceItem { get; set; }
    }
    
    public enum PickupStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        PickedUp = 4,
        Completed = 5,
        Cancelled = 6
    }
}
