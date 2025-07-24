using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Dtos;
using Services.Features.Notifications.Repositories;
using Services.Features.Instagram.Repositories;

namespace Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de notificações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ApiControllerBase
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IInstagramRepository _instagramRepo;
        private readonly ILogger<NotificationController> _logger;
        
        // ID do usuário de teste - apenas para desenvolvimento
        private static readonly Guid TestUserId = Guid.Parse("7C808BCE-F73E-4885-A19B-E7271C4BBF20");

        /// <summary>
        /// Inicializa uma nova instância do controlador de notificações
        /// </summary>
        /// <param name="notificationRepo">Repositório de notificações</param>
        /// <param name="instagramRepo">Repositório do Instagram</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public NotificationController(
            INotificationRepository notificationRepo,
            IInstagramRepository instagramRepo,
            ILogger<NotificationController> logger)
        {
            _notificationRepo = notificationRepo;
            _instagramRepo = instagramRepo;
            _logger = logger;
        }

        /// <summary>
        /// Retorna as notificações do usuário, podendo incluir notificações já lidas.
        /// </summary>
        /// <param name="includeRead">Se verdadeiro, inclui notificações lidas.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Lista de notificações do usuário.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserNotifications([FromQuery] bool includeRead = false)
        {
            // TODO: Pegar userId do token de autenticação quando implementar autenticação
            var userId = TestUserId; // ID do usuário de teste

            var notifications = await _notificationRepo.GetUserNotificationsAsync(userId, includeRead);
            var posts = await _instagramRepo.GetPostsByIdsAsync(notifications.Select(n => n.PostId));

            var dtos = notifications.Join(
                posts,
                n => n.PostId,
                p => p.Id,
                (n, p) => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    PostId = n.PostId,
                    Topic = p.Topic ?? "",
                    SentAt = n.SentAt,
                    IsRead = n.IsRead,
                    Content = new PostContentRequest
                    {
                        Id = p.Id,
                        Text = p.Caption ?? "",
                        ImageUrl = p.DisplayUrl ?? p.Images,
                        Url = p.Url,
                        PostType = p.Type ?? "",
                        CreatedAt = p.CreatedAt
                    }
                });

            return Success(dtos);
        }

        /// <summary>
        /// Retorna as preferências do usuário para notificações.
        /// </summary>
        /// <returns>
        /// <para><b>200 OK</b>: Lista de preferências.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpGet("preferences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPreferences()
        {
            // TODO: Pegar userId do token de autenticação quando implementar autenticação
            var userId = TestUserId; // ID do usuário de teste

            var preferences = await _notificationRepo.GetPreferencesAsync(userId);
            return Success(preferences);
        }

        /// <summary>
        /// Atualiza as preferências de notificação do usuário.
        /// </summary>
        /// <param name="request">Lista de tópicos de interesse.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Preferências atualizadas com sucesso.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPut("preferences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdatePreferencesRequest request)
        {
            // TODO: Pegar userId do token de autenticação quando implementar autenticação
            var userId = TestUserId; // ID do usuário de teste

            await _notificationRepo.UpdatePreferencesAsync(userId, request.Topics);
            return Success();
        }

        /// <summary>
        /// Marca uma notificação específica como lida.
        /// </summary>
        /// <param name="id">ID da notificação.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Notificação marcada como lida.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPut("{id}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            // TODO: Pegar userId do token de autenticação quando implementar autenticação
            var userId = TestUserId; // ID do usuário de teste

            await _notificationRepo.MarkAsReadAsync(userId, id);
            return Success();
        }

        /// <summary>
        /// Marca todas as notificações do usuário como lidas.
        /// </summary>
        /// <returns>
        /// <para><b>200 OK</b>: Todas as notificações marcadas como lidas.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            // TODO: Pegar userId do token de autenticação quando implementar autenticação
            var userId = TestUserId; // ID do usuário de teste

            await _notificationRepo.MarkAllAsReadAsync(userId);
            return Success();
        }
    }
}
