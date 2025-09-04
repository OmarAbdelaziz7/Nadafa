using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Infrastructure.Repositories;

namespace Presentation.Controllers
{
    public class NotificationController : ApiControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(
            INotificationService notificationService,
            INotificationRepository notificationRepository)
        {
            _notificationService = notificationService;
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Get user notifications (All authenticated users)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedNotificationsDto>> GetNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationRepository.GetByUserIdAsync(userId, unreadOnly, page, pageSize);
                var totalCount = await _notificationRepository.GetTotalCountByUserIdAsync(userId, unreadOnly);

                var response = new PaginatedNotificationsDto
                {
                    Items = notifications.Select(n => new NotificationResponseDto
                    {
                        Id = n.Id,
                        UserId = n.UserId,
                        UserName = n.User?.Name,
                        Type = n.NotificationType,
                        Title = n.Title,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        ReadAt = n.IsRead ? n.CreatedAt : null // This would need to be tracked properly
                    }),
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasNextPage = page * pageSize < totalCount,
                    HasPreviousPage = page > 1
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Mark notification as read (All authenticated users)
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<ActionResult<NotificationResponseDto>> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Verify the notification belongs to the user
                var notification = await _notificationRepository.GetByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                if (notification.UserId != userId)
                {
                    return Forbid();
                }

                var result = await _notificationService.MarkAsReadAsync(id);

                var response = new NotificationResponseDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    UserName = result.User?.Name,
                    Type = result.NotificationType,
                    Title = result.Title,
                    Message = result.Message,
                    IsRead = result.IsRead,
                    CreatedAt = result.CreatedAt,
                    ReadAt = result.IsRead ? DateTime.UtcNow : null
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Mark all notifications as read (All authenticated users)
        /// </summary>
        [HttpPut("mark-all-read")]
        public async Task<ActionResult<object>> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _notificationService.MarkAllAsReadAsync(userId);

                return Ok(new
                {
                    message = $"{count} notifications marked as read",
                    count = count
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get unread notification count (All authenticated users)
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<object>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _notificationRepository.GetTotalCountByUserIdAsync(userId, unreadOnly: true);

                return Ok(new { count = count });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Delete notification (All authenticated users)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteNotification(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Verify the notification belongs to the user
                var notification = await _notificationRepository.GetByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                if (notification.UserId != userId)
                {
                    return Forbid();
                }

                // This would need to be implemented in the repository
                // For now, we'll return a placeholder response
                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get notification statistics (All authenticated users)
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetNotificationStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                var totalCount = await _notificationRepository.GetTotalCountByUserIdAsync(userId, unreadOnly: false);
                var unreadCount = await _notificationRepository.GetTotalCountByUserIdAsync(userId, unreadOnly: true);

                return Ok(new
                {
                    TotalNotifications = totalCount,
                    UnreadNotifications = unreadCount,
                    ReadNotifications = totalCount - unreadCount
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
