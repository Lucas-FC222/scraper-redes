using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Core.Repositories;
using Core.Models;

namespace Data
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

        public async Task<IEnumerable<SentNotification>> GetUserNotificationsAsync(Guid userId, bool includeRead = false)
        {
            var sql = @"
                SELECT NotificationId, UserId, PostId, SentAt, IsRead
                FROM SentNotifications
                WHERE UserId = @UserId";

            if (!includeRead)
            {
                sql += " AND IsRead = 0";
            }

            sql += " ORDER BY SentAt DESC";

            await _connection.OpenAsync();
            var notifications = await _connection.QueryAsync<SentNotification>(sql, new { UserId = userId });
            await _connection.CloseAsync();

            _logger.LogInformation("GetUserNotificationsAsync({UserId}): {Count} notificações",
                userId, notifications.Count());
            return notifications;
        }

        public async Task UpdatePreferencesAsync(Guid userId, string[] topics)
        {
            const string deleteSql = @"
                DELETE FROM UserTopicPreferences
                WHERE UserId = @UserId";

            const string insertSql = @"
                INSERT INTO UserTopicPreferences (UserId, Topic)
                VALUES (@UserId, @Topic)";

            await _connection.OpenAsync();
            using var tx = _connection.BeginTransaction();
            try
            {
                await _connection.ExecuteAsync(deleteSql, new { UserId = userId }, tx);

                foreach (var topic in topics)
                {
                    await _connection.ExecuteAsync(insertSql, new { UserId = userId, Topic = topic }, tx);
                }

                tx.Commit();
                _logger.LogInformation("UpdatePreferencesAsync({UserId}): atualizados {Count} tópicos",
                    userId, topics.Length);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            const string sql = @"
                UPDATE SentNotifications
                SET IsRead = 1
                WHERE UserId = @UserId
                AND NotificationId = @NotificationId";

            await _connection.OpenAsync();
            var affected = await _connection.ExecuteAsync(sql, new { UserId = userId, NotificationId = notificationId });
            await _connection.CloseAsync();

            _logger.LogInformation("MarkAsReadAsync({UserId}, {NotificationId}): {Affected} registros atualizados",
                userId, notificationId, affected);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            const string sql = @"
                UPDATE SentNotifications
                SET IsRead = 1
                WHERE UserId = @UserId
                AND IsRead = 0";

            await _connection.OpenAsync();
            var affected = await _connection.ExecuteAsync(sql, new { UserId = userId });
            await _connection.CloseAsync();

            _logger.LogInformation("MarkAllAsReadAsync({UserId}): {Affected} registros atualizados",
                userId, affected);
        }
    }
}
