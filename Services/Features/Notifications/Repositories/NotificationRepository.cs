using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Services.Features.Auth.Models;
using Services.Features.Notifications.Models;

namespace Services.Features.Notifications.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento de notificações e preferências de usuários na base de dados.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        /// <summary>
        /// Conexão com o banco de dados SQL Server.
        /// </summary>
        private readonly SqlConnection _connection;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<NotificationRepository> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="NotificationRepository"/>.
        /// </summary>
        /// <param name="connection">Conexão SQL para acesso ao banco de dados.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public NotificationRepository(SqlConnection connection, ILogger<NotificationRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os usuários cadastrados.
        /// </summary>
        /// <returns>Coleção de usuários da aplicação.</returns>
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

        /// <summary>
        /// Obtém as preferências de tópicos de um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Coleção de tópicos de interesse.</returns>
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

        /// <summary>
        /// Verifica se o usuário já foi notificado sobre um post.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="postId">Identificador do post.</param>
        /// <returns>True se já foi notificado, caso contrário false.</returns>
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

        /// <summary>
        /// Marca que o usuário foi notificado sobre um post.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="postId">Identificador do post.</param>
        public async Task MarkNotifiedAsync(Guid userId, string postId)
        {
            const string sql = @"
                INSERT INTO SentNotifications (UserId, PostId)
                VALUES (@UserId, @PostId)";

            await _connection.OpenAsync();
            using var tx = await _connection.BeginTransactionAsync();
            await _connection.ExecuteAsync(sql, new { UserId = userId, PostId = postId }, tx);
            await tx.CommitAsync();
            await _connection.CloseAsync();

            _logger.LogInformation("MarkNotifiedAsync: registro criado para {UserId}/{PostId}", userId, postId);
        }

        /// <summary>
        /// Obtém os identificadores dos posts para os quais o usuário já foi notificado.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Coleção de identificadores de posts notificados.</returns>
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

        /// <summary>
        /// Obtém as notificações enviadas para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="includeRead">Se true, inclui notificações já lidas.</param>
        /// <returns>Coleção de notificações enviadas.</returns>
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

        /// <summary>
        /// Atualiza as preferências de tópicos de um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="topics">Array de tópicos de interesse.</param>
        public async Task UpdatePreferencesAsync(Guid userId, string[] topics)
        {
            const string deleteSql = @"
                DELETE FROM UserTopicPreferences
                WHERE UserId = @UserId";

            const string insertSql = @"
                INSERT INTO UserTopicPreferences (UserId, Topic)
                VALUES (@UserId, @Topic)";

            await _connection.OpenAsync();
            using var tx = await _connection.BeginTransactionAsync();
            await _connection.ExecuteAsync(deleteSql, new { UserId = userId }, tx);

            foreach (var topic in topics)
            {
                await _connection.ExecuteAsync(insertSql, new { UserId = userId, Topic = topic }, tx);
            }

            await tx.CommitAsync();
            _logger.LogInformation("UpdatePreferencesAsync({UserId}): atualizados {Count} tópicos",
                userId, topics.Length);
            await _connection.CloseAsync();
        }

        /// <summary>
        /// Marca uma notificação como lida para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="notificationId">Identificador da notificação.</param>
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

        /// <summary>
        /// Marca todas as notificações como lidas para um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
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
