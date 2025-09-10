using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NadafaDbContext _context;

        public NotificationRepository(NadafaDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, bool unreadOnly, int page, int pageSize)
        {
            var query = _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Notification> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.MarkAsRead();
                await _context.SaveChangesAsync();
            }
            return notification;
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            await _context.SaveChangesAsync();
            return unreadNotifications.Count();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId, bool unreadOnly)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query.CountAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}
