using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User
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

        [Range(18, 120)]
        public int Age { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        public Role Role { get; set; } = Role.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties for relationships
        public virtual ICollection<Payment> Payments { get; set; }
        // New navigation properties - will be added in separate migration
        public virtual ICollection<PickupRequest> PickupRequests { get; set; }
        public virtual ICollection<MarketplaceItem> MarketplaceItems { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<PickupRequest> AdminApprovedRequests { get; set; }
    }
}
