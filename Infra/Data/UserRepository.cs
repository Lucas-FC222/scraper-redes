using Core.Models;
using Core.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Infra.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            SqlConnection connection,
            ILogger<UserRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<AppUser> CreateAsync(AppUser user)
        {
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO AppUsers (UserId, Email, Name, Password, Role)
                VALUES (@UserId, @Email, @Name, @Password, @Role);
            ";

            user.UserId = Guid.NewGuid();
            await _connection.ExecuteAsync(sql, user);

            // Inserir as preferências de tópicos
            if (user.TopicPreferences != null && user.TopicPreferences.Any())
            {
                var topicSql = @"
                    INSERT INTO UserTopicPreferences (UserId, Topic)
                    VALUES (@UserId, @Topic);
                ";

                foreach (var topic in user.TopicPreferences)
                {
                    await _connection.ExecuteAsync(topicSql, new { UserId = user.UserId, Topic = topic });
                }
            }

            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _connection.OpenAsync();

            // Deletar preferências de tópicos primeiro
            var deleteTopicsSql = "DELETE FROM UserTopicPreferences WHERE UserId = @UserId";
            await _connection.ExecuteAsync(deleteTopicsSql, new { UserId = id });

            // Depois deletar o usuário
            var deleteUserSql = "DELETE FROM AppUsers WHERE UserId = @UserId";
            await _connection.ExecuteAsync(deleteUserSql, new { UserId = id });
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            await _connection.OpenAsync();

            var sql = "SELECT COUNT(1) FROM AppUsers WHERE Email = @Email";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Email = email });

            return count > 0;
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            await _connection.OpenAsync();

            var users = await _connection.QueryAsync<AppUser>("SELECT * FROM AppUsers");

            foreach (var user in users)
            {
                // Carregar preferências de tópicos para cada usuário
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { UserId = user.UserId });
                user.TopicPreferences = topics.ToList();
            }

            return users;
        }

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM AppUsers WHERE Email = @Email";
            var user = await _connection.QueryFirstOrDefaultAsync<AppUser>(sql, new { Email = email });

            if (user != null)
            {
                // Carregar preferências de tópicos
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { UserId = user.UserId });
                user.TopicPreferences = topics.ToList();
            }

            return user;
        }

        public async Task<AppUser> GetByIdAsync(Guid id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM AppUsers WHERE UserId = @UserId";
            var user = await _connection.QueryFirstOrDefaultAsync<AppUser>(sql, new { UserId = id });

            if (user != null)
            {
                // Carregar preferências de tópicos
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { UserId = user.UserId });
                user.TopicPreferences = topics.ToList();
            }

            return user;
        }

        public async Task UpdateAsync(AppUser user)
        {
            await _connection.OpenAsync();

            var sql = @"
                UPDATE AppUsers 
                SET Email = @Email, 
                    Name = @Name, 
                    Password = @Password,
                    Role = @Role
                WHERE UserId = @UserId
            ";

            await _connection.ExecuteAsync(sql, user);

            // Atualizar preferências de tópicos
            // Primeiro remover todas as existentes
            var deleteTopicsSql = "DELETE FROM UserTopicPreferences WHERE UserId = @UserId";
            await _connection.ExecuteAsync(deleteTopicsSql, new { UserId = user.UserId });

            // Depois inserir as novas
            if (user.TopicPreferences != null && user.TopicPreferences.Any())
            {
                var topicSql = @"
                    INSERT INTO UserTopicPreferences (UserId, Topic)
                    VALUES (@UserId, @Topic);
                ";

                foreach (var topic in user.TopicPreferences)
                {
                    await _connection.ExecuteAsync(topicSql, new { UserId = user.UserId, Topic = topic });
                }
            }
        }
    }
}
