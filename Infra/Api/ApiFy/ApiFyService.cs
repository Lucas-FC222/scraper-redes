using System.Text.Json;
using System.Net.Http.Json;
using Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Infra
{
    public class ApiFyService : IApiFyService
    {
        private readonly HttpClient _httpClient;
        private readonly ApifySettings _settings;
        private readonly ILogger<ApiFyService> _logger;

        public ApiFyService(HttpClient httpClient, IOptions<ApifySettings> settings, ILogger<ApiFyService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }



        public async Task<string?> RunFacebookScraperAsync(string pageUrl, int maxPosts)
        {
            _logger.LogInformation("Iniciando scraper do Facebook para página: {PageUrl}, limite: {MaxPosts}", pageUrl, maxPosts);
            var input = new { query = pageUrl, max_posts = maxPosts, search_type = "posts" };
            var startUrl = $"https://api.apify.com/v2/acts/l6CUZt8H0214D3I0N/runs?token={_settings.ApiToken}";
            var startResp = await _httpClient.PostAsJsonAsync(startUrl, input);
            if (!startResp.IsSuccessStatusCode) 
            {
                _logger.LogError("Erro ao iniciar scraper do Facebook. Status: {StatusCode}", startResp.StatusCode);
                return null;
            }
            using var startJson = await JsonDocument.ParseAsync(await startResp.Content.ReadAsStreamAsync());
            var runId = startJson.RootElement.GetProperty("data").GetProperty("id").GetString();
            _logger.LogInformation("Scraper do Facebook iniciado com sucesso. RunId: {RunId}", runId);
            return runId;
        }

        public async Task<Core.FacebookDataResult?> ProcessFacebookDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Iniciando processamento do dataset do Facebook: {DatasetId}", datasetId);
            var itemsUrl = $"https://api.apify.com/v2/datasets/{datasetId}/items?token={_settings.ApiToken}";
            _logger.LogInformation("Fazendo requisição para: {Url}", itemsUrl.Replace(_settings.ApiToken, "***"));
            var itemsResp = await _httpClient.GetAsync(itemsUrl);
            _logger.LogInformation("Resposta da API Apify - Status: {StatusCode}", itemsResp.StatusCode);
            if (!itemsResp.IsSuccessStatusCode)
            {
                var errorContent = await itemsResp.Content.ReadAsStringAsync();
                _logger.LogError("Erro na requisição para Apify. Status: {StatusCode}, Content: {Content}", 
                    itemsResp.StatusCode, errorContent);
                return null;
            }
            var json = await itemsResp.Content.ReadAsStringAsync();
            _logger.LogInformation("JSON recebido da API Apify. Tamanho: {Size} caracteres", json.Length);
            var preview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
            _logger.LogInformation("Preview do JSON: {Preview}", preview);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apifyItems = JsonSerializer.Deserialize<IEnumerable<Core.ApifyFacebookPost>>(json, options);
            if (apifyItems == null)
            {
                _logger.LogError("Falha na desserialização do JSON para ApifyFacebookPost");
                return null;
            }
            
            _logger.LogInformation("Desserialização bem-sucedida. {Count} itens encontrados", apifyItems.Count());
            
            // Mapear ApifyFacebookPost para FacebookPost
            var facebookPosts = apifyItems.Select(MapToFacebookPost).ToList();
            _logger.LogInformation("Mapeamento concluído. {PostsCount} posts do Facebook mapeados", facebookPosts.Count);
            
            return new Core.FacebookDataResult
            {
                Posts = facebookPosts
            };
        }

        public Task<FacebookPost> GetPostFacebookAsync(string postId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<InstagramPost> GetPostInstagramAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> RunInstagramScraperAsync(string username, int limit)
        {
            _logger.LogInformation("Iniciando scraper do Instagram para username: {Username}, limite: {Limit}", username, limit);
            var input = new { directUrls = new[] { $"https://www.instagram.com/{username}/" }, resultsLimit = limit };
            var startUrl = $"https://api.apify.com/v2/acts/{_settings.ActorId}/runs?token={_settings.ApiToken}";
            var startResp = await _httpClient.PostAsJsonAsync(startUrl, input);
            if (!startResp.IsSuccessStatusCode) 
            {
                _logger.LogError("Erro ao iniciar scraper. Status: {StatusCode}", startResp.StatusCode);
                return null;
            }
            using var startJson = await JsonDocument.ParseAsync(await startResp.Content.ReadAsStreamAsync());
            var runId = startJson.RootElement.GetProperty("data").GetProperty("id").GetString();
            _logger.LogInformation("Scraper iniciado com sucesso. RunId: {RunId}", runId);
            return runId;
        }

        public async Task<Core.InstagramDataResult?> ProcessInstagramDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Iniciando processamento do dataset: {DatasetId}", datasetId);
            var itemsUrl = $"https://api.apify.com/v2/datasets/{datasetId}/items?token={_settings.ApiToken}";
            _logger.LogInformation("Fazendo requisição para: {Url}", itemsUrl.Replace(_settings.ApiToken, "***"));
            var itemsResp = await _httpClient.GetAsync(itemsUrl);
            _logger.LogInformation("Resposta da API Apify - Status: {StatusCode}", itemsResp.StatusCode);
            if (!itemsResp.IsSuccessStatusCode)
            {
                var errorContent = await itemsResp.Content.ReadAsStringAsync();
                _logger.LogError("Erro na requisição para Apify. Status: {StatusCode}, Content: {Content}", 
                    itemsResp.StatusCode, errorContent);
                return null;
            }
            var json = await itemsResp.Content.ReadAsStringAsync();
            _logger.LogInformation("JSON recebido da API Apify. Tamanho: {Size} caracteres", json.Length);
            var preview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
            _logger.LogInformation("Preview do JSON: {Preview}", preview);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apifyItems = JsonSerializer.Deserialize<IEnumerable<Core.ApifyInstagramPost>>(json, options);
            if (apifyItems == null)
            {
                _logger.LogError("Falha na desserialização do JSON para ApifyInstagramPost");
                return null;
            }
            
            _logger.LogInformation("Desserialização bem-sucedida. {Count} itens encontrados", apifyItems.Count());
            
            // Mapear ApifyInstagramPost para InstagramPost e dados relacionados
            var instagramPosts = apifyItems.Select(MapToInstagramPost).ToList();
            var allComments = apifyItems.SelectMany(MapToComments).ToList();
            var allHashtags = apifyItems.SelectMany(MapToHashtags).ToList();
            var allMentions = apifyItems.SelectMany(MapToMentions).ToList();
            
            _logger.LogInformation("Mapeamento concluído. {PostsCount} posts, {CommentsCount} comentários, {HashtagsCount} hashtags, {MentionsCount} menções", 
                instagramPosts.Count, allComments.Count, allHashtags.Count, allMentions.Count);
            
            // Retornar um objeto com todos os dados
            return new InstagramDataResult
            {
                Posts = instagramPosts,
                Comments = allComments,
                Hashtags = allHashtags,
                Mentions = allMentions
            };
        }

        private Core.FacebookPost MapToFacebookPost(Core.ApifyFacebookPost apifyPost)
        {
            return new Core.FacebookPost
            {
                Id = apifyPost.PostId,
                Url = apifyPost.Url,
                Message = apifyPost.Message,
                Timestamp = apifyPost.Timestamp,
                CommentsCount = apifyPost.CommentsCount,
                ReactionsCount = apifyPost.ReactionsCount,
                AuthorId = apifyPost.Author?.Id ?? string.Empty,
                AuthorName = apifyPost.Author?.Name ?? string.Empty,
                AuthorUrl = apifyPost.Author?.Url ?? string.Empty,
                AuthorProfilePictureUrl = apifyPost.Author?.ProfilePictureUrl ?? string.Empty,
                Image = apifyPost.Image != null ? System.Text.Json.JsonSerializer.Serialize(apifyPost.Image) : string.Empty,
                Video = apifyPost.Video != null ? System.Text.Json.JsonSerializer.Serialize(apifyPost.Video) : string.Empty,
                AttachedPostUrl = apifyPost.AttachedPostUrl != null ? System.Text.Json.JsonSerializer.Serialize(apifyPost.AttachedPostUrl) : string.Empty,
                PageUrl = string.Empty, // Será preenchido pelo serviço
                CreatedAt = DateTime.UtcNow
            };
        }

        private Core.InstagramPost MapToInstagramPost(Core.ApifyInstagramPost apifyPost)
        {
            return new Core.InstagramPost
            {
                Id = apifyPost.Id,
                Type = apifyPost.Type,
                ShortCode = apifyPost.ShortCode,
                Caption = apifyPost.Caption,
                Url = apifyPost.Url,
                CommentsCount = apifyPost.CommentsCount,
                DimensionsHeight = apifyPost.DimensionsHeight,
                DimensionsWidth = apifyPost.DimensionsWidth,
                DisplayUrl = apifyPost.DisplayUrl,
                Images = System.Text.Json.JsonSerializer.Serialize(apifyPost.Images),
                VideoUrl = apifyPost.VideoUrl,
                Alt = apifyPost.Alt ?? string.Empty,
                LikesCount = apifyPost.LikesCount,
                VideoViewCount = apifyPost.VideoViewCount,
                VideoPlayCount = apifyPost.VideoPlayCount,
                Timestamp = apifyPost.Timestamp,
                ChildPosts = System.Text.Json.JsonSerializer.Serialize(apifyPost.ChildPosts),
                OwnerFullName = apifyPost.OwnerFullName,
                OwnerUsername = apifyPost.OwnerUsername,
                OwnerId = apifyPost.OwnerId,
                ProductType = apifyPost.ProductType,
                VideoDuration = apifyPost.VideoDuration,
                IsSponsored = apifyPost.IsSponsored,
                TaggedUsers = System.Text.Json.JsonSerializer.Serialize(apifyPost.TaggedUsers),
                MusicInfo = apifyPost.MusicInfo != null ? System.Text.Json.JsonSerializer.Serialize(apifyPost.MusicInfo) : string.Empty,
                CoauthorProducers = System.Text.Json.JsonSerializer.Serialize(apifyPost.CoauthorProducers),
                IsCommentsDisabled = apifyPost.IsCommentsDisabled,
                InputUrl = apifyPost.InputUrl,
                CreatedAt = DateTime.UtcNow
            };
        }

        private IEnumerable<Core.InstagramComment> MapToComments(Core.ApifyInstagramPost apifyPost)
        {
            return apifyPost.LatestComments.Select(comment => new Core.InstagramComment
            {
                Id = comment.Id,
                PostId = apifyPost.Id,
                Text = comment.Text,
                OwnerUsername = comment.OwnerUsername,
                OwnerId = comment.Owner?.Id ?? string.Empty,
                OwnerProfilePicUrl = comment.OwnerProfilePicUrl,
                Timestamp = comment.Timestamp,
                RepliesCount = comment.RepliesCount,
                LikesCount = comment.LikesCount,
                Replies = System.Text.Json.JsonSerializer.Serialize(comment.Replies),
                CreatedAt = DateTime.UtcNow
            });
        }

        private IEnumerable<Core.InstagramHashtag> MapToHashtags(Core.ApifyInstagramPost apifyPost)
        {
            return apifyPost.Hashtags.Select(hashtag => new Core.InstagramHashtag
            {
                PostId = apifyPost.Id,
                Hashtag = hashtag,
                CreatedAt = DateTime.UtcNow
            });
        }

        private IEnumerable<Core.InstagramMention> MapToMentions(Core.ApifyInstagramPost apifyPost)
        {
            return apifyPost.TaggedUsers.Select(user => new Core.InstagramMention
            {
                PostId = apifyPost.Id,
                MentionedUsername = user.Username,
                MentionedUserId = user.Id,
                MentionedFullName = user.FullName,
                MentionedProfilePicUrl = user.ProfilePicUrl,
                IsVerified = user.IsVerified,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    public interface IApiFyService
    {
        Task<FacebookPost> GetPostFacebookAsync(string postId, CancellationToken cancellationToken);
        Task<InstagramPost> GetPostInstagramAsync(string id, CancellationToken cancellationToken);
        Task<string?> RunInstagramScraperAsync(string username, int limit);
        Task<string?> RunFacebookScraperAsync(string pageUrl, int maxPosts);
        Task<Core.InstagramDataResult?> ProcessInstagramDatasetAsync(string datasetId);
        Task<Core.FacebookDataResult?> ProcessFacebookDatasetAsync(string datasetId);
    }
}