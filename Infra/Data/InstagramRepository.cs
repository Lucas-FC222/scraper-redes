using Dapper;
using Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Infra.Data
{
    public class InstagramRepository : IInstagramRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<InstagramRepository> _logger;

        public InstagramRepository(SqlConnection connection, ILogger<InstagramRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<IEnumerable<InstagramPost>> GetPostsByIdsAsync(IEnumerable<string> ids)
        {
            if (!ids.Any())
                return Enumerable.Empty<InstagramPost>();

            const string sql = @"
                SELECT Id, Type, Caption, DisplayUrl, Images, Url, CreatedAt, Topic
                FROM InstagramPosts
                WHERE Id IN @Ids";

            await _connection.OpenAsync();
            var posts = await _connection.QueryAsync<InstagramPost>(sql, new { Ids = ids });
            await _connection.CloseAsync();

            _logger.LogInformation("GetPostsByIdsAsync: {Count} posts encontrados", posts.Count());
            return posts;
        }

        public async Task SavePostsAsync(IEnumerable<InstagramPost> posts)
        {
            try
            {
                var postsList = posts.ToList();
                // Garante que todos os itens tenham CreatedAt preenchido
                foreach (var post in postsList)
                {
                    if (post.CreatedAt == default)
                        post.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} posts no banco de dados", postsList.Count);

                const string sql = @"
                    INSERT INTO InstagramPosts
                        (Id, Type, ShortCode, Caption, Url, CommentsCount, DimensionsHeight, DimensionsWidth, DisplayUrl, Images, 
                         VideoUrl, Alt, LikesCount, VideoViewCount, VideoPlayCount, Timestamp, ChildPosts, OwnerFullName, OwnerUsername, 
                         OwnerId, ProductType, VideoDuration, IsSponsored, TaggedUsers, MusicInfo, CoauthorProducers, IsCommentsDisabled, 
                         InputUrl, CreatedAt)
                    VALUES
                        (@Id, @Type, @ShortCode, @Caption, @Url, @CommentsCount, @DimensionsHeight, @DimensionsWidth, @DisplayUrl, @Images, 
                         @VideoUrl, @Alt, @LikesCount, @VideoViewCount, @VideoPlayCount, @Timestamp, @ChildPosts, @OwnerFullName, @OwnerUsername, 
                         @OwnerId, @ProductType, @VideoDuration, @IsSponsored, @TaggedUsers, @MusicInfo, @CoauthorProducers, @IsCommentsDisabled, 
                         @InputUrl, @CreatedAt)
                ";

                await _connection.OpenAsync();
                _logger.LogInformation("Conexão com banco aberta");

                using var tx = _connection.BeginTransaction();
                _logger.LogInformation("Transação iniciada");

                var rowsAffected = await _connection.ExecuteAsync(sql, postsList, transaction: tx);
                _logger.LogInformation("ExecuteAsync concluído. {RowsAffected} linhas afetadas", rowsAffected);

                tx.Commit();
                _logger.LogInformation("Transação commitada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar posts no banco de dados");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                    _logger.LogInformation("Conexão com banco fechada");
                }
            }
        }

        public async Task<IEnumerable<InstagramPost>> GetAllPostsAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todos os posts do banco");
                
                const string sql = @"
                    SELECT Id, Type, ShortCode, Caption, Url, CommentsCount, DimensionsHeight, DimensionsWidth, DisplayUrl, Images, 
                           VideoUrl, Alt, LikesCount, VideoViewCount, VideoPlayCount, Timestamp, ChildPosts, OwnerFullName, OwnerUsername, 
                           OwnerId, ProductType, VideoDuration, IsSponsored, TaggedUsers, MusicInfo, CoauthorProducers, IsCommentsDisabled, 
                           InputUrl, CreatedAt, Topic
                    FROM InstagramPosts
                    ORDER BY CreatedAt DESC
                ";

                await _connection.OpenAsync();
                var results = await _connection.QueryAsync<InstagramPost>(sql);
                _logger.LogInformation("Encontrados {Count} posts", results.Count());
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os posts");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task<InstagramPost?> GetPostByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Buscando post com ID: {Id}", id);
                
                const string sql = @"
                    SELECT Id, Type, ShortCode, Caption, Url, CommentsCount, DimensionsHeight, DimensionsWidth, DisplayUrl, Images, 
                           VideoUrl, Alt, LikesCount, VideoViewCount, VideoPlayCount, Timestamp, ChildPosts, OwnerFullName, OwnerUsername, 
                           OwnerId, ProductType, VideoDuration, IsSponsored, TaggedUsers, MusicInfo, CoauthorProducers, IsCommentsDisabled, 
                           InputUrl, CreatedAt, Topic
                    FROM InstagramPosts
                    WHERE Id = @Id
                ";

                await _connection.OpenAsync();
                var result = await _connection.QueryFirstOrDefaultAsync<InstagramPost>(sql, new { Id = id });
                
                if (result != null)
                {
                    _logger.LogInformation("Post encontrado para ID: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Post não encontrado para ID: {Id}", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post com ID: {Id}", id);
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task SaveCommentsAsync(IEnumerable<InstagramComment> comments)
        {
            try
            {
                var commentsList = comments.ToList();
                foreach (var comment in commentsList)
                {
                    if (comment.CreatedAt == default)
                        comment.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} comentários no banco de dados", commentsList.Count);

                const string sql = @"
                    INSERT INTO InstagramComments
                        (Id, PostId, Text, OwnerUsername, OwnerId, OwnerProfilePicUrl, Timestamp, RepliesCount, LikesCount, Replies, CreatedAt)
                    VALUES
                        (@Id, @PostId, @Text, @OwnerUsername, @OwnerId, @OwnerProfilePicUrl, @Timestamp, @RepliesCount, @LikesCount, @Replies, @CreatedAt)
                ";

                await _connection.OpenAsync();
                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(sql, commentsList, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Comentários salvos com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar comentários no banco de dados");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task SaveHashtagsAsync(IEnumerable<InstagramHashtag> hashtags)
        {
            try
            {
                var hashtagsList = hashtags.ToList();
                foreach (var hashtag in hashtagsList)
                {
                    if (hashtag.CreatedAt == default)
                        hashtag.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} hashtags no banco de dados", hashtagsList.Count);

                const string sql = @"
                    INSERT INTO InstagramHashtags
                        (PostId, Hashtag, CreatedAt)
                    VALUES
                        (@PostId, @Hashtag, @CreatedAt)
                ";

                await _connection.OpenAsync();
                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(sql, hashtagsList, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Hashtags salvas com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar hashtags no banco de dados");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task SaveMentionsAsync(IEnumerable<InstagramMention> mentions)
        {
            try
            {
                var mentionsList = mentions.ToList();
                foreach (var mention in mentionsList)
                {
                    if (mention.CreatedAt == default)
                        mention.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} menções no banco de dados", mentionsList.Count);

                const string sql = @"
                    INSERT INTO InstagramMentions
                        (PostId, MentionedUsername, MentionedUserId, MentionedFullName, MentionedProfilePicUrl, IsVerified, CreatedAt)
                    VALUES
                        (@PostId, @MentionedUsername, @MentionedUserId, @MentionedFullName, @MentionedProfilePicUrl, @IsVerified, @CreatedAt)
                ";

                await _connection.OpenAsync();
                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(sql, mentionsList, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Menções salvas com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar menções no banco de dados");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando comentários do post: {PostId}", postId);
                
                const string sql = @"
                    SELECT Id, PostId, Text, OwnerUsername, OwnerId, OwnerProfilePicUrl, Timestamp, RepliesCount, LikesCount, Replies, CreatedAt
                    FROM InstagramComments
                    WHERE PostId = @PostId
                    ORDER BY CreatedAt DESC
                ";

                await _connection.OpenAsync();
                var results = await _connection.QueryAsync<InstagramComment>(sql, new { PostId = postId });
                _logger.LogInformation("Encontrados {Count} comentários para o post {PostId}", results.Count(), postId);
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar comentários do post {PostId}", postId);
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task<IEnumerable<InstagramHashtag>> GetHashtagsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando hashtags do post: {PostId}", postId);
                
                const string sql = @"
                    SELECT PostId, Hashtag, CreatedAt
                    FROM InstagramHashtags
                    WHERE PostId = @PostId
                    ORDER BY CreatedAt DESC
                ";

                await _connection.OpenAsync();
                var results = await _connection.QueryAsync<InstagramHashtag>(sql, new { PostId = postId });
                _logger.LogInformation("Encontradas {Count} hashtags para o post {PostId}", results.Count(), postId);
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hashtags do post {PostId}", postId);
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

        public async Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando menções do post: {PostId}", postId);
                
                const string sql = @"
                    SELECT PostId, MentionedUsername, MentionedUserId, MentionedFullName, MentionedProfilePicUrl, IsVerified, CreatedAt
                    FROM InstagramMentions
                    WHERE PostId = @PostId
                    ORDER BY CreatedAt DESC
                ";

                await _connection.OpenAsync();
                var results = await _connection.QueryAsync<InstagramMention>(sql, new { PostId = postId });
                _logger.LogInformation("Encontradas {Count} menções para o post {PostId}", results.Count(), postId);
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar menções do post {PostId}", postId);
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }
    }
} 