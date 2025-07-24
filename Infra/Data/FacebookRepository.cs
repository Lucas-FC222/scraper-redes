using Core.Models;
using Core.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FacebookRepository : IFacebookRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<FacebookRepository> _logger;

        public FacebookRepository(SqlConnection connection, ILogger<FacebookRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task SavePostsAsync(IEnumerable<FacebookPost> posts)
        {
            var postsList = posts.ToList();
            foreach (var post in postsList)
            {
                if (post.CreatedAt == default)
                    post.CreatedAt = DateTime.UtcNow;
            }

            _logger.LogInformation("Iniciando salvamento de {Count} posts do Facebook no banco de dados", postsList.Count);

            const string sql = @"
                INSERT INTO FacebookPosts
                    (Id, Url, Message, Timestamp, CommentsCount, ReactionsCount, AuthorId, AuthorName, AuthorUrl, AuthorProfilePictureUrl,
                     Image, Video, AttachedPostUrl, PageUrl, CreatedAt, Topic)
                VALUES
                    (@Id, @Url, @Message, @Timestamp, @CommentsCount, @ReactionsCount, @AuthorId, @AuthorName, @AuthorUrl, @AuthorProfilePictureUrl,
                     @Image, @Video, @AttachedPostUrl, @PageUrl, @CreatedAt, @Topic)
            ";

            await _connection.OpenAsync();
            using var tx = _connection.BeginTransaction();
            var rowsAffected = await _connection.ExecuteAsync(sql, postsList, transaction: tx);
            tx.Commit();
            _logger.LogInformation("Posts do Facebook salvos com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
            
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<FacebookPost>> GetAllPostsAsync()
        {
            _logger.LogInformation("Buscando todos os posts do Facebook");
            
            const string sql = @"
                SELECT Id, Url, Message, Timestamp, CommentsCount, ReactionsCount, AuthorId, AuthorName, AuthorUrl, AuthorProfilePictureUrl,
                       Image, Video, AttachedPostUrl, PageUrl, CreatedAt, Topic
                FROM FacebookPosts
                ORDER BY CreatedAt DESC
            ";

            await _connection.OpenAsync();
            var results = await _connection.QueryAsync<FacebookPost>(sql);
            _logger.LogInformation("Encontrados {Count} posts do Facebook", results.Count());
            
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
            
            return results;
        }

        public async Task<FacebookPost?> GetPostByIdAsync(string id)
        {
            _logger.LogInformation("Buscando post do Facebook com ID: {Id}", id);
            
            const string sql = @"
                SELECT Id, Url, Message, Timestamp, CommentsCount, ReactionsCount, AuthorId, AuthorName, AuthorUrl, AuthorProfilePictureUrl,
                       Image, Video, AttachedPostUrl, PageUrl, CreatedAt, Topic
                FROM FacebookPosts
                WHERE Id = @Id
            ";

            await _connection.OpenAsync();
            var result = await _connection.QueryFirstOrDefaultAsync<FacebookPost>(sql, new { Id = id });
            
            if (result != null)
            {
                _logger.LogInformation("Post do Facebook encontrado para ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Post do Facebook não encontrado para ID: {Id}", id);
            }
            
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
            
            return result;
        }

        public async Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            if (keywords == null || !keywords.Any())
                return Enumerable.Empty<FacebookPost>();

            _logger.LogInformation("Buscando posts do Facebook por palavras-chave: {Keywords}", string.Join(", ", keywords));

            // Monta o filtro dinâmico para as palavras-chave
            var filters = keywords.Select((k, i) => $"LOWER(Message) LIKE @kw{i}").ToList();
            var whereClause = string.Join(" OR ", filters);
            var sql = $@"
                SELECT Id, Url, Message, Timestamp, CommentsCount, ReactionsCount, AuthorId, AuthorName, AuthorUrl, AuthorProfilePictureUrl,
                       Image, Video, AttachedPostUrl, PageUrl, CreatedAt
                FROM FacebookPosts
                WHERE {whereClause}
                ORDER BY CreatedAt DESC
            ";

            var parameters = new DynamicParameters();
            int idx = 0;
            foreach (var kw in keywords)
            {
                parameters.Add($"kw{idx}", $"%{kw.ToLower()}%");
                idx++;
            }

            await _connection.OpenAsync();
            var results = await _connection.QueryAsync<FacebookPost>(sql, parameters);
            _logger.LogInformation("Encontrados {Count} posts do Facebook por palavras-chave", results.Count());
            
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
            
            return results;
        }
    }
} 