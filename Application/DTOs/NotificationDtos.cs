using System;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs
{
    public class NotificationDto
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }

    public class CreateNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
    }

    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class PaginatedNotificationsDto
    {
        public System.Collections.Generic.IEnumerable<NotificationResponseDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class MarkNotificationAsReadDto
    {
        [Required]
        public int NotificationId { get; set; }
    }

    public class MarkAllNotificationsAsReadDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
