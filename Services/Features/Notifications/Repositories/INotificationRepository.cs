using Services.Features.Auth.Models;
using Services.Features.Notifications.Models;

namespace Services.Features.Notifications.Repositories
{
    /// <summary>
    /// Interface responsável pelo gerenciamento de notificações e preferências de usuários.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Obtém todos os usuários cadastrados.
        /// </summary>
        /// <returns>Coleção de usuários da aplicação.</returns>
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        /// <summary>
        /// Obtém as preferências de tópicos de um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Coleção de tópicos de interesse.</returns>
        Task<IEnumerable<string>> GetPreferencesAsync(Guid userId);
        /// <summary>
        /// Verifica se o usuário já foi notificado sobre um post.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>True se já foi notificado, caso contrário false.</returns>
        Task<bool> WasNotifiedAsync(Guid userId, string postId);
        /// <summary>
        /// Marca que o usuário foi notificado sobre um post.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="postId">Identificador do post.</param>
        Task MarkNotifiedAsync(Guid userId, string postId);
        /// <summary>
        /// Obtém os identificadores dos posts para os quais o usuário já foi notificado.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Coleção de identificadores de posts notificados.</returns>
        Task<IEnumerable<string>> GetNotifiedPostIdsAsync(Guid userId);
        /// <summary>
        /// Obtém as notificações enviadas para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="includeRead">Se true, inclui notificações já lidas.</param>
        /// <returns>Coleção de notificações enviadas.</returns>
        Task<IEnumerable<SentNotification>> GetUserNotificationsAsync(Guid userId, bool includeRead = false);
        /// <summary>
        /// Atualiza as preferências de tópicos de um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="topics">Array de tópicos de interesse.</param>
        Task UpdatePreferencesAsync(Guid userId, string[] topics);
        /// <summary>
        /// Marca uma notificação como lida para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="notificationId">Identificador da notificação.</param>
        Task MarkAsReadAsync(Guid userId, Guid notificationId);
        /// <summary>
        /// Marca todas as notificações como lidas para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        Task MarkAllAsReadAsync(Guid userId);
    }

}
