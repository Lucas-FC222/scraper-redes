//using Microsoft.AspNetCore.Mvc;
//using Shared.Domain.Dtos;
//using Services.Features.Notifications.Repositories;
//using Services.Features.Instagram.Repositories;

//namespace Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class NotificationController : ApiControllerBase
//    {
//        private readonly INotificationRepository _notificationRepo;
//        private readonly IInstagramRepository _instagramRepo;
//        private readonly ILogger<NotificationController> _logger;
        
//        // ID do usuário de teste - apenas para desenvolvimento
//        private static readonly Guid TestUserId = Guid.Parse("7C808BCE-F73E-4885-A19B-E7271C4BBF20");

//        public NotificationController(
//            INotificationRepository notificationRepo,
//            IInstagramRepository instagramRepo,
//            ILogger<NotificationController> logger)
//        {
//            _notificationRepo = notificationRepo;
//            _instagramRepo = instagramRepo;
//            _logger = logger;
//        }

//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> GetUserNotifications([FromQuery] bool includeRead = false)
//        {
//            // TODO: Pegar userId do token de autenticação quando implementar autenticação
//            var userId = TestUserId; // ID do usuário de teste

//            var notifications = await _notificationRepo.GetUserNotificationsAsync(userId, includeRead);
//            var posts = await _instagramRepo.GetPostsByIdsAsync(notifications.Select(n => n.PostId));

//            var dtos = notifications.Join(
//                posts,
//                n => n.PostId,
//                p => p.Id,
//                (n, p) => new NotificationDto
//                {
//                    NotificationId = n.NotificationId,
//                    PostId = n.PostId,
//                    Topic = p.Topic ?? "",
//                    SentAt = n.SentAt,
//                    IsRead = n.IsRead,
//                    Content = new PostContentRequest
//                    {
//                        Id = p.Id,
//                        Text = p.Caption ?? "",
//                        ImageUrl = p.DisplayUrl ?? p.Images,
//                        Url = p.Url,
//                        PostType = p.Type ?? "",
//                        CreatedAt = p.CreatedAt
//                    }
//                });

//            return Success(dtos);
//        }

//        [HttpGet("preferences")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> GetUserPreferences()
//        {
//            // TODO: Pegar userId do token de autenticação quando implementar autenticação
//            var userId = TestUserId; // ID do usuário de teste

//            var preferences = await _notificationRepo.GetPreferencesAsync(userId);
//            return Success(preferences);
//        }

//        [HttpPut("preferences")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdatePreferencesRequest request)
//        {
//            // TODO: Pegar userId do token de autenticação quando implementar autenticação
//            var userId = TestUserId; // ID do usuário de teste

//            await _notificationRepo.UpdatePreferencesAsync(userId, request.Topics);
//            return Success();
//        }

//        [HttpPut("{id}/read")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> MarkAsRead(Guid id)
//        {
//            // TODO: Pegar userId do token de autenticação quando implementar autenticação
//            var userId = TestUserId; // ID do usuário de teste

//            await _notificationRepo.MarkAsReadAsync(userId, id);
//            return Success();
//        }

//        [HttpPut("read-all")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> MarkAllAsRead()
//        {
//            // TODO: Pegar userId do token de autenticação quando implementar autenticação
//            var userId = TestUserId; // ID do usuário de teste

//            await _notificationRepo.MarkAllAsReadAsync(userId);
//            return Success();
//        }
//    }
//}
