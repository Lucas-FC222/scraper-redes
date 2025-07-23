using Core;

namespace Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Envia notificação (por ex. e‑mail) para o usuário
        /// </summary>
        Task SendAsync(AppUser user, Core.InstagramPost post);
    }

}
