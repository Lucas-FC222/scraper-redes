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
    }

}
