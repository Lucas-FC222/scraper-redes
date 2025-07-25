using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Models;

namespace Services.Features.Facebook.Repositories
{
    /// <summary>
    /// Repositório responsável pelo acesso e manipulação de dados de posts do Facebook na base de dados.
    /// </summary>
    public class FacebookRepository : IFacebookRepository
    {
        /// <summary>
        /// Conexão com o banco de dados SQL Server.
        /// </summary>
        private readonly SqlConnection _connection;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<FacebookRepository> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="FacebookRepository"/>.
        /// </summary>
        /// <param name="connection">Conexão SQL para acesso ao banco de dados.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public FacebookRepository(SqlConnection connection, ILogger<FacebookRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// Salva uma coleção de posts do Facebook no banco de dados.
        /// </summary>
        /// <param name="posts">Coleção de posts a serem salvos.</param>
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
            using var tx = await _connection.BeginTransactionAsync();
            var rowsAffected = await _connection.ExecuteAsync(sql, postsList, transaction: tx);
            await tx.CommitAsync();
            _logger.LogInformation("Posts do Facebook salvos com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
            await _connection.CloseAsync();
        }

        /// <summary>
        /// Obtém todos os posts do Facebook cadastrados no banco de dados.
        /// </summary>
        /// <returns>Coleção de posts do Facebook.</returns>
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
            await _connection.CloseAsync();
            return results;
        }

        /// <summary>
        /// Obtém um post do Facebook pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do post.</param>
        /// <returns>Post do Facebook correspondente ao ID informado ou null se não encontrado.</returns>
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

            await _connection.CloseAsync();
            return result;
        }

        /// <summary>
        /// Pesquisa posts do Facebook por uma lista de palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para pesquisa nos posts.</param>
        /// <returns>Coleção de posts que correspondem às palavras-chave.</returns>
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
            await _connection.CloseAsync();
            return results;
        }
    }
}