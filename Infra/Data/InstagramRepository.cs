using Core.Models;
using Core.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Data
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
            var postsList = posts.ToList();
            if (!postsList.Any()) return;

            // Garante que todos os itens tenham CreatedAt preenchido
            foreach (var post in postsList)
            {
                if (post.CreatedAt == default)
                    post.CreatedAt = DateTime.UtcNow;
            }

            try
            {
                await _connection.OpenAsync();
                var postIds = postsList.Select(p => p.Id).ToList();

                const string checkSql = "SELECT Id FROM InstagramPosts WHERE Id IN @Ids";
                var existingPostIds = (await _connection.QueryAsync<string>(checkSql, new { Ids = postIds })).ToHashSet();

                if (existingPostIds.Any())
                {
                    _logger.LogInformation("{Count} posts já existem no banco de dados e serão ignorados.", existingPostIds.Count);
                }

                var newPosts = postsList.Where(p => !existingPostIds.Contains(p.Id)).ToList();
                if (!newPosts.Any())
                {
                    _logger.LogInformation("Nenhum post novo para ser salvo.");
                    return;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} novos posts no banco de dados", newPosts.Count);
                const string insertSql = @"
                    INSERT INTO InstagramPosts (Id, Type, ShortCode, Caption, Url, CommentsCount, DimensionsHeight, DimensionsWidth, DisplayUrl, Images, VideoUrl, Alt, LikesCount, VideoViewCount, VideoPlayCount, Timestamp, ChildPosts, OwnerFullName, OwnerUsername, OwnerId, ProductType, VideoDuration, IsSponsored, TaggedUsers, MusicInfo, CoauthorProducers, IsCommentsDisabled, InputUrl, CreatedAt, Topic)
                    VALUES (@Id, @Type, @ShortCode, @Caption, @Url, @CommentsCount, @DimensionsHeight, @DimensionsWidth, @DisplayUrl, @Images, @VideoUrl, @Alt, @LikesCount, @VideoViewCount, @VideoPlayCount, @Timestamp, @ChildPosts, @OwnerFullName, @OwnerUsername, @OwnerId, @ProductType, @VideoDuration, @IsSponsored, @TaggedUsers, @MusicInfo, @CoauthorProducers, @IsCommentsDisabled, @InputUrl, @CreatedAt, @Topic)";

                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(insertSql, newPosts, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Novos posts salvos com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
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
            var commentsList = comments.ToList();
            if (!commentsList.Any()) return;

            foreach (var comment in commentsList)
            {
                if (comment.CreatedAt == default)
                    comment.CreatedAt = DateTime.UtcNow;
            }

            try
            {
                await _connection.OpenAsync();
                var commentIds = commentsList.Select(c => c.Id).ToList();

                const string checkSql = "SELECT Id FROM InstagramComments WHERE Id IN @Ids";
                var existingCommentIds = (await _connection.QueryAsync<string>(checkSql, new { Ids = commentIds })).ToHashSet();

                if (existingCommentIds.Any())
                {
                    _logger.LogInformation("{Count} comentários já existem no banco de dados e serão ignorados.", existingCommentIds.Count);
                }

                var newComments = commentsList.Where(c => !existingCommentIds.Contains(c.Id)).ToList();

                if (!newComments.Any())
                {
                    _logger.LogInformation("Nenhum comentário novo para ser salvo.");
                    return;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} novos comentários no banco de dados", newComments.Count);
                const string insertSql = @"
                    INSERT INTO InstagramComments (Id, PostId, Text, OwnerUsername, OwnerId, OwnerProfilePicUrl, Timestamp, RepliesCount, LikesCount, Replies, CreatedAt)
                    VALUES (@Id, @PostId, @Text, @OwnerUsername, @OwnerId, @OwnerProfilePicUrl, @Timestamp, @RepliesCount, @LikesCount, @Replies, @CreatedAt)";

                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(insertSql, newComments, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Novos comentários salvos com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
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
            var hashtagsList = hashtags.ToList();
            if (!hashtagsList.Any()) return;

            foreach (var hashtag in hashtagsList)
            {
                if (hashtag.CreatedAt == default)
                    hashtag.CreatedAt = DateTime.UtcNow;
            }

            try
            {
                await _connection.OpenAsync();

                var postIds = hashtagsList.Select(h => h.PostId).Distinct().ToList();

                const string checkSql = "SELECT PostId, Hashtag FROM InstagramHashtags WHERE PostId IN @PostIds";
                var existingHashtags = (await _connection.QueryAsync<(string PostId, string Hashtag)>(checkSql, new { PostIds = postIds }))
                                        .ToHashSet();

                var newHashtags = hashtagsList.Where(h => !existingHashtags.Contains((h.PostId, h.Hashtag))).ToList();

                if (!newHashtags.Any())
                {
                    _logger.LogInformation("Nenhuma hashtag nova para ser salva.");
                    return;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} novas hashtags no banco de dados", newHashtags.Count);
                const string insertSql = @"
                    INSERT INTO InstagramHashtags (PostId, Hashtag, CreatedAt)
                    VALUES (@PostId, @Hashtag, @CreatedAt)";

                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(insertSql, newHashtags, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Novas hashtags salvas com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
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
            var mentionsList = mentions.ToList();
            if (!mentionsList.Any()) return;

            foreach (var mention in mentionsList)
            {
                if (mention.CreatedAt == default)
                    mention.CreatedAt = DateTime.UtcNow;
            }

            try
            {
                await _connection.OpenAsync();

                var postIds = mentionsList.Select(m => m.PostId).Distinct().ToList();

                const string checkSql = "SELECT PostId, MentionedUsername FROM InstagramMentions WHERE PostId IN @PostIds";
                var existingMentions = (await _connection.QueryAsync<(string PostId, string MentionedUsername)>(checkSql, new { PostIds = postIds }))
                                        .ToHashSet();

                var newMentions = mentionsList.Where(m => !existingMentions.Contains((m.PostId, m.MentionedUsername))).ToList();

                if (!newMentions.Any())
                {
                    _logger.LogInformation("Nenhuma menção nova para ser salva.");
                    return;
                }

                _logger.LogInformation("Iniciando salvamento de {Count} novas menções no banco de dados", newMentions.Count);
                const string insertSql = @"
                    INSERT INTO InstagramMentions (PostId, MentionedUsername, MentionedUserId, MentionedFullName, MentionedProfilePicUrl, IsVerified, CreatedAt)
                    VALUES (@PostId, @MentionedUsername, @MentionedUserId, @MentionedFullName, @MentionedProfilePicUrl, @IsVerified, @CreatedAt)";

                using var tx = _connection.BeginTransaction();
                var rowsAffected = await _connection.ExecuteAsync(insertSql, newMentions, transaction: tx);
                tx.Commit();
                _logger.LogInformation("Novas menções salvas com sucesso. {RowsAffected} linhas afetadas", rowsAffected);
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

        public async Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            if (keywords == null || !keywords.Any())
                return Enumerable.Empty<InstagramPost>();

            try
            {
                _logger.LogInformation("Buscando posts do Instagram por palavras-chave: {Keywords}", string.Join(", ", keywords));

                var filters = keywords.Select((k, i) => $"LOWER(Caption) LIKE @kw{i}").ToList();
                var whereClause = string.Join(" OR ", filters);
                var sql = $@"
                    SELECT Id, Type, ShortCode, Caption, Url, CommentsCount, DimensionsHeight, DimensionsWidth, DisplayUrl, Images, 
                           VideoUrl, Alt, LikesCount, VideoViewCount, VideoPlayCount, Timestamp, ChildPosts, OwnerFullName, OwnerUsername, 
                           OwnerId, ProductType, VideoDuration, IsSponsored, TaggedUsers, MusicInfo, CoauthorProducers, IsCommentsDisabled, 
                           InputUrl, CreatedAt, Topic
                    FROM InstagramPosts
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
                var results = await _connection.QueryAsync<InstagramPost>(sql, parameters);
                _logger.LogInformation("Encontrados {Count} posts do Instagram por palavras-chave", results.Count());
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts do Instagram por palavras-chave");
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