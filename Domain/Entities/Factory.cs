using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Factory
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        [StringLength(100)]
        public string BusinessLicense { get; set; }
        
        public Role Role { get; set; } = Role.Factory;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsVerified { get; set; } = false;
        
        // Navigation properties for relationships
        public virtual ICollection<MarketplaceItem> PurchasedItems { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
