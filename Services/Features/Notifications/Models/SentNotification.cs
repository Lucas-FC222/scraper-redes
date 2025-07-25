namespace Services.Features.Notifications.Models
{
    /// <summary>
    /// Representa uma notificação enviada para um usuário referente a um post.
    /// </summary>
    public class SentNotification
    {
        /// <summary>
        /// Identificador único da notificação.
        /// </summary>
        public Guid NotificationId { get; set; }
        /// <summary>
        /// Identificador do usuário que recebeu a notificação.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Identificador do post relacionado à notificação.
        /// </summary>
        public string PostId { get; set; } = "";
        /// <summary>
        /// Data e hora em que a notificação foi enviada.
        /// </summary>
        public DateTime SentAt { get; set; }
        /// <summary>
        /// Indica se a notificação foi lida pelo usuário.
        /// </summary>
        public bool IsRead { get; set; }
    }

}
