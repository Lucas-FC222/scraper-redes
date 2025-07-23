using Dapper;
using Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Infra
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(
            SqlConnection connection,
            ILogger<NotificationRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            const string sql = @"
                SELECT UserId, Email, Name
                FROM AppUsers";

            await _connection.OpenAsync();
            var users = await _connection.QueryAsync<AppUser>(sql);
            await _connection.CloseAsync();

            _logger.LogInformation("GetAllUsersAsync: {Count} usuários", users.Count());
            return users;
        }

        public async Task<IEnumerable<string>> GetPreferencesAsync(Guid userId)
        {
            const string sql = @"
                SELECT Topic
                FROM UserTopicPreferences
                WHERE UserId = @UserId";

            await _connection.OpenAsync();
            var topics = await _connection.QueryAsync<string>(sql, new { UserId = userId });
            await _connection.CloseAsync();

            _logger.LogInformation("GetPreferencesAsync({UserId}): {Count} tópicos",
                userId, topics.Count());
            return topics;
        }

        public async Task<bool> WasNotifiedAsync(Guid userId, string postId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM SentNotifications
                WHERE UserId = @UserId
                  AND PostId = @PostId";

            await _connection.OpenAsync();
            int count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, PostId = postId });
            await _connection.CloseAsync();

            bool was = count > 0;
            _logger.LogDebug("WasNotifiedAsync({UserId},{PostId}) = {Was}", userId, postId, was);
            return was;
        }

        public async Task MarkNotifiedAsync(Guid userId, string postId)
        {
            const string sql = @"
                INSERT INTO SentNotifications (UserId, PostId)
                VALUES (@UserId, @PostId)";

            await _connection.OpenAsync();
            using var tx = _connection.BeginTransaction();
            await _connection.ExecuteAsync(sql, new { UserId = userId, PostId = postId }, tx);
            tx.Commit();
            await _connection.CloseAsync();

            _logger.LogInformation("MarkNotifiedAsync: registro criado para {UserId}/{PostId}", userId, postId);
        }

        public async Task<IEnumerable<string>> GetNotifiedPostIdsAsync(Guid userId)
        {
            const string sql = @"
                    SELECT PostId
                      FROM SentNotifications
                     WHERE UserId = @UserId";
            await _connection.OpenAsync();
            var ids = await _connection.QueryAsync<string>(sql, new { UserId = userId });
            await _connection.CloseAsync();
            return ids;
        }
    }
}
