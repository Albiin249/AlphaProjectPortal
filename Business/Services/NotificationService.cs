using Business.Interfaces;
using Data.Contexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class NotificationService(AppDbContext context) : INotificationService
{
    private readonly AppDbContext _context = context;

    public async Task AddNotificationAsync(NotificationEntity notificationEntity)
    {
        if (string.IsNullOrEmpty(notificationEntity.Icon))
        {
            switch (notificationEntity.NotificationTypeId)
            {
                case 1:
                    notificationEntity.Icon = "/images/users/user-template-male.svg";
                    break;
                case 2:
                    notificationEntity.Icon = "/images/projects/project-template.svg";
                    break;
            }
        }

        _context.Add(notificationEntity);
        await _context.SaveChangesAsync();

    }

    public async Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10)
    {
        var dismissedIds = await _context.DismissedNotifications
            .Where(x => x.UserId == userId)
            .Select(x => x.NotificationId)
            .ToListAsync();

        var notifications = await _context.Notifications
            .Where(x => !dismissedIds.Contains(x.Id))
            .OrderByDescending(x => x.Created)
            .Take(take)
            .ToListAsync();

        return notifications;
    }

    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var alreadyDismissed = await _context.DismissedNotifications.AnyAsync(x => x.NotificationId == notificationId && x.UserId == userId);
        if (!alreadyDismissed)
        {
            var dismissed = new NotificationDismissedEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };

            _context.Add(dismissed);
            await _context.SaveChangesAsync();
        }
    }
}
