using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(int userId, string title, string message, NotificationType type);
        Task<Notification> MarkAsReadAsync(Guid notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
        Task<Notification> GetByIdAsync(Guid id);
    }
}
