using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, bool unreadOnly, int page, int pageSize);
        Task<Notification> MarkAsReadAsync(Guid notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
        Task<int> GetTotalCountByUserIdAsync(int userId, bool unreadOnly);
        Task<Notification> GetByIdAsync(Guid id);
    }
}
