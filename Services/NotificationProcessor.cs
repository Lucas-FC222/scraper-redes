using Microsoft.Extensions.Logging;
using Core.Repositories;
using Core.Services;

namespace Services
{
    public class NotificationProcessor : INotificationProcessorService
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IInstagramRepository _postRepo;
        //private readonly INotificationService _notifier;
        private readonly ILogger<NotificationProcessor> _logger;

        public NotificationProcessor(
            INotificationRepository notifRepo,
            IInstagramRepository postRepo,
            //INotificationService notifier,
            ILogger<NotificationProcessor> logger)
        {
            _notifRepo = notifRepo;
            _postRepo = postRepo;
           // _notifier = notifier;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var users = await _notifRepo.GetAllUsersAsync();
            foreach (var user in users)
            {
                if (cancellationToken.IsCancellationRequested) break;

                var topics = (await _notifRepo.GetPreferencesAsync(user.UserId))
                .Select(t => t.Trim().ToLowerInvariant())
                .ToHashSet();
                var sentPostIds = (await _notifRepo.GetNotifiedPostIdsAsync(user.UserId))
                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!topics.Any()) continue;

                var allPosts = await _postRepo.GetAllPostsAsync();
                foreach (var p in allPosts)
                {
                    _logger.LogDebug("Post {PostId} tem Topic raw: '{TopicRaw}'", p.Id, p.Topic);
                }
                var newPosts = allPosts
                    .Where(p => topics.Contains((p.Topic ?? "")
                              .Trim()
                              .ToLowerInvariant()))
                    .Where(p => !sentPostIds.Contains(p.Id));

                _logger.LogInformation(
                        $"Usuário {user.Email}: temas [{string.Join(",", topics)}]; já notificados [{sentPostIds.Count}]; posts totais [{allPosts.Count()}]; novos [{newPosts.Count()}]");

                foreach (var post in newPosts)
                {
                    //await _notifier.SendAsync(user, post);
                    await _notifRepo.MarkNotifiedAsync(user.UserId, post.Id);
                    _logger.LogInformation("User {Email} notified of post {PostId}", user.Email, post.Id);
                }
            }
        }
    }
}
