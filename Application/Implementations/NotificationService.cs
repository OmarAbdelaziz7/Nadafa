using System;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> CreateNotificationAsync(int userId, string title, string message, NotificationType type)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = type,
                IsRead = false
            };

            return await _notificationRepository.CreateAsync(notification);
        }

        public async Task<Notification> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.MarkAsReadAsync(notificationId);
            if (notification == null)
                throw new ArgumentException("Notification not found");

            return notification;
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                throw new ArgumentException("Notification not found");

            return notification;
        }
    }
}
