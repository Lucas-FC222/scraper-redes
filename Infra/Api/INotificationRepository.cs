using Core;

namespace Infra
{
    public interface INotificationRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<IEnumerable<string>> GetPreferencesAsync(Guid userId);
        Task<bool> WasNotifiedAsync(Guid userId, string postId);
        Task MarkNotifiedAsync(Guid userId, string postId);
        Task<IEnumerable<string>> GetNotifiedPostIdsAsync(Guid userId);

        // Novos métodos para suporte à API
        Task<IEnumerable<SentNotification>> GetUserNotificationsAsync(Guid userId, bool includeRead = false);
        Task UpdatePreferencesAsync(Guid userId, string[] topics);
        Task MarkAsReadAsync(Guid userId, Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
    }

}
