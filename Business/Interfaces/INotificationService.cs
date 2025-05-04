using Data.Entities;

namespace Business.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(NotificationEntity notificationEntity);
        Task DismissNotificationAsync(string notificationId, string userId);
        Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10);
    }
}