using Shared.Domain.Dtos;

namespace Services.Features.Notifications.Models
{
    /// <summary>
    /// Representa uma notificação enviada ao usuário
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// Identificador único da notificação
        /// </summary>
        public Guid NotificationId { get; set; }
        
        /// <summary>
        /// Identificador do post relacionado à notificação
        /// </summary>
        public string PostId { get; set; } = "";
        
        /// <summary>
        /// Tópico ou categoria da notificação
        /// </summary>
        public string Topic { get; set; } = "";
        
        /// <summary>
        /// Data e hora em que a notificação foi enviada
        /// </summary>
        public DateTime SentAt { get; set; }
        
        /// <summary>
        /// Indica se a notificação foi lida pelo usuário
        /// </summary>
        public bool IsRead { get; set; }
        
        /// <summary>
        /// Conteúdo do post relacionado à notificação
        /// </summary>
        public PostContentRequest Content { get; set; } = null!;
    }




}
