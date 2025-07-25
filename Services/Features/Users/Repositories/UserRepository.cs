using Dapper;
using Microsoft.Data.SqlClient;
using Services.Features.Auth.Models;

namespace Services.Features.Users.Repositories
{
    /// <summary>
    /// Repositório de usuários para operações CRUD e preferências de tópicos.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Conexão com o banco de dados SQL Server.
        /// </summary>
        private readonly SqlConnection _connection;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="connection">Conexão SQL para acesso ao banco de dados.</param>
        public UserRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Cria um novo usuário e suas preferências de tópicos.
        /// </summary>
        /// <param name="user">Usuário a ser criado.</param>
        /// <returns>Usuário criado.</returns>
        public async Task<AppUser?> CreateAsync(AppUser user)
        {
            await _connection.OpenAsync();

            var sql = @"
                INSERT INTO AppUsers (UserId, Email, Name, Password, Role)
                VALUES (@UserId, @Email, @Name, @Password, @Role);
            ";

            user.UserId = Guid.NewGuid();
            await _connection.ExecuteAsync(sql, user);

            // Inserir as preferências de tópicos
            if (user.TopicPreferences != null && user.TopicPreferences.Count != 0)
            {
                var topicSql = @"
                    INSERT INTO UserTopicPreferences (UserId, Topic)
                    VALUES (@UserId, @Topic);
                ";

                foreach (var topic in user.TopicPreferences)
                {
                    await _connection.ExecuteAsync(topicSql, new { user.UserId, Topic = topic });
                }
            }

            return user;
        }

        /// <summary>
        /// Remove um usuário e suas preferências de tópicos.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
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

        /// <summary>
        /// Verifica se existe um usuário com o email informado.
        /// </summary>
        /// <param name="email">Email a ser verificado.</param>
        /// <returns>True se existir, false caso contrário.</returns>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            await _connection.OpenAsync();

            var sql = "SELECT COUNT(1) FROM AppUsers WHERE Email = @Email";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Email = email });

            return count > 0;
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            await _connection.OpenAsync();

            var users = await _connection.QueryAsync<AppUser>("SELECT * FROM AppUsers");

            foreach (var user in users)
            {
                // Carregar preferências de tópicos para cada usuário
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { user.UserId });
                user.TopicPreferences = [.. topics];
            }

            return users;
        }

        /// <summary>
        /// Busca um usuário pelo email.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM AppUsers WHERE Email = @Email";
            var user = await _connection.QueryFirstOrDefaultAsync<AppUser>(sql, new { Email = email });

            if (user != null)
            {
                // Carregar preferências de tópicos
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { user.UserId });
                user.TopicPreferences = [.. topics];
            }

            return user;
        }

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        public async Task<AppUser?> GetByIdAsync(Guid id)
        {
            await _connection.OpenAsync();

            var sql = "SELECT * FROM AppUsers WHERE UserId = @UserId";
            var user = await _connection.QueryFirstOrDefaultAsync<AppUser>(sql, new { UserId = id });

            if (user != null)
            {
                // Carregar preferências de tópicos
                var topicsSql = "SELECT Topic FROM UserTopicPreferences WHERE UserId = @UserId";
                var topics = await _connection.QueryAsync<string>(topicsSql, new { user.UserId });
                user.TopicPreferences = [.. topics];
            }

            return user;
        }

        /// <summary>
        /// Atualiza os dados e preferências de um usuário.
        /// </summary>
        /// <param name="user">Usuário a ser atualizado.</param>
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
            await _connection.ExecuteAsync(deleteTopicsSql, new { user.UserId });

            // Depois inserir as novas
            if (user.TopicPreferences != null && user.TopicPreferences.Count != 0)
            {
                var topicSql = @"
                    INSERT INTO UserTopicPreferences (UserId, Topic)
                    VALUES (@UserId, @Topic);
                ";

                foreach (var topic in user.TopicPreferences)
                {
                    await _connection.ExecuteAsync(topicSql, new { user.UserId, Topic = topic });
                }
            }
        }
    }
}
