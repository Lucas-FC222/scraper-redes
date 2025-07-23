using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.Models;
using Core;
using Infra;
using Infra.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IInstagramRepository _instagramRepo;
        private readonly ILogger<NotificationController> _logger;
        
        // ID do usuário de teste - apenas para desenvolvimento
        private static readonly Guid TestUserId = Guid.Parse("7C808BCE-F73E-4885-A19B-E7271C4BBF20");

        public NotificationController(
            INotificationRepository notificationRepo,
            IInstagramRepository instagramRepo,
            ILogger<NotificationController> logger)
        {
            _notificationRepo = notificationRepo;
            _instagramRepo = instagramRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications([FromQuery] bool includeRead = false)
        {
            try
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
                        Content = new PostContentDto
                        {
                            Id = p.Id,
                            Text = p.Caption ?? "",
                            ImageUrl = p.DisplayUrl ?? p.Images,
                            Url = p.Url,
                            PostType = p.Type ?? "",
                            CreatedAt = p.CreatedAt
                        }
                    });

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPreferences()
        {
            try
            {
                // TODO: Pegar userId do token de autenticação quando implementar autenticação
                var userId = TestUserId; // ID do usuário de teste

                var preferences = await _notificationRepo.GetPreferencesAsync(userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting preferences");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("preferences")]
        public async Task<ActionResult> UpdateUserPreferences([FromBody] UpdatePreferencesDto request)
        {
            try
            {
                // TODO: Pegar userId do token de autenticação quando implementar autenticação
                var userId = TestUserId; // ID do usuário de teste

                await _notificationRepo.UpdatePreferencesAsync(userId, request.Topics);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preferences");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                // TODO: Pegar userId do token de autenticação quando implementar autenticação
                var userId = TestUserId; // ID do usuário de teste

                await _notificationRepo.MarkAsReadAsync(userId, id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                // TODO: Pegar userId do token de autenticação quando implementar autenticação
                var userId = TestUserId; // ID do usuário de teste

                await _notificationRepo.MarkAllAsReadAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
