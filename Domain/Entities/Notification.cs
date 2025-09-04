using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; }

        // Helper method to mark as read
        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
