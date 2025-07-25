using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Features.Instagram.Models;
using Shared.Domain.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Services.Features.Instagram.Externals.Api.Client
{
    /// <summary>
    /// Implementação do cliente responsável por integrar com a API Apify para operações de scraping e processamento de dados do Instagram.
    /// </summary>
    public class ApifyInstagramClient : IApifyInstagramClient
    {
        /// <summary>
        /// Instância de HttpClient utilizada para requisições HTTP à API Apify.
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// Logger para registro de eventos e informações de execução.
        /// </summary>
        private readonly ILogger<ApifyInstagramClient> _logger;
        /// <summary>
        /// Configurações da Apify injetadas via IOptions.
        /// </summary>
        private readonly IOptions<Models.ApifySettings> _settings;
        /// <summary>
        /// Opções de serialização JSON para desserialização case-insensitive.
        /// </summary>
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ApifyInstagramClient"/>.
        /// </summary>
        /// <param name="httpClient">Instância de HttpClient para requisições HTTP.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="settings">Configurações da Apify.</param>
        public ApifyInstagramClient(HttpClient httpClient, ILogger<ApifyInstagramClient> logger, IOptions<Models.ApifySettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        /// Inicia o scraper do Instagram para um username específico.
        /// </summary>
        /// <param name="username">Username do Instagram.</param>
        /// <param name="limit">Limite de posts a coletar.</param>
        /// <returns>Resultado contendo o RunId do scraper ou erro.</returns>
        public async Task<Result<string>> RunInstagramScraperAsync(string username, int limit)
        {
            _logger.LogInformation("Iniciando scraper do Instagram para username: {Username}, limite: {Limit}", username, limit);

            var input = new { directUrls = new[] { $"https://www.instagram.com/{username}/" }, resultsLimit = limit };
            var startUrl = $"https://api.apify.com/v2/acts/{_settings.Value.ActorId}/runs?token={_settings.Value.ApiToken}";

            var startResp = await _httpClient.PostAsJsonAsync(startUrl, input);
            if (!startResp.IsSuccessStatusCode)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = $"Erro ao iniciar scraper do Instagram. Status: {startResp.StatusCode}",
                    Status = (int)startResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, startResp.StatusCode);

                return Result<string>.Fail(problemDetails);
            }

            using var startJson = await JsonDocument.ParseAsync(await startResp.Content.ReadAsStreamAsync());
            var runId = startJson.RootElement.GetProperty("data").GetProperty("id").GetString();

            if (string.IsNullOrEmpty(runId))
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = $"RunId não pode ser nulo ou vazio. Status: {startResp.StatusCode}",
                    Status = (int)startResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, startResp.StatusCode);

                return Result<string>.Fail(problemDetails);
            }

            _logger.LogInformation("Scraper do Instagram iniciado com sucesso. RunId: {RunId}", runId);

            return Result<string>.Ok(runId);
        }

        /// <summary>
        /// Processa o dataset do Instagram retornado pela Apify.
        /// </summary>
        /// <param name="datasetId">ID do dataset.</param>
        /// <returns>Resultado contendo os dados do Instagram processados ou erro.</returns>
        public async Task<Result<InstagramData>> ProcessInstagramDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Iniciando processamento do dataset: {DatasetId}", datasetId);

            var itemsUrl = $"https://api.apify.com/v2/datasets/{datasetId}/items?token={_settings.Value.ApiToken}";
            _logger.LogInformation("Fazendo requisição para: {Url}", itemsUrl.Replace(_settings.Value.ApiToken, "***"));

            var itemsResp = await _httpClient.GetAsync(itemsUrl);
            _logger.LogInformation("Resposta da API Apify - Status: {StatusCode}", itemsResp.StatusCode);

            if (!itemsResp.IsSuccessStatusCode)
            {
                var errorContent = await itemsResp.Content.ReadAsStringAsync();

                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = $"Erro na requisição para Apify. Status: {itemsResp.StatusCode}, Content: {errorContent}",
                    Status = (int)itemsResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, itemsResp.StatusCode);

                return Result<InstagramData>.Fail(problemDetails);
            }

            var json = await itemsResp.Content.ReadAsStringAsync();
            _logger.LogInformation("JSON recebido da API Apify. Tamanho: {Size} caracteres", json.Length);

            var preview = json.Length > 500 ? string.Concat(json.AsSpan(0, 500), "...") : json;
            _logger.LogInformation("Preview do JSON: {Preview}", preview);

            var apifyItems = JsonSerializer.Deserialize<IEnumerable<ApifyInstagramPost>>(json, _jsonSerializerOptions);
            if (apifyItems == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = "Falha na desserialização do JSON para ApifyInstagramPost",
                    Status = (int)itemsResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, itemsResp.StatusCode);

                return Result<InstagramData>.Fail(problemDetails);
            }

            _logger.LogInformation("Desserialização bem-sucedida. {Count} itens encontrados", apifyItems.Count());

            var instagramPosts = apifyItems.Select(MapToInstagramPost).ToList();
            var allComments = apifyItems.SelectMany(MapToComments).ToList();
            var allHashtags = apifyItems.SelectMany(MapToHashtags).ToList();
            var allMentions = apifyItems.SelectMany(MapToMentions).ToList();

            _logger.LogInformation("Mapeamento concluído. {PostsCount} posts, {CommentsCount} comentários, {HashtagsCount} hashtags, {MentionsCount} menções", instagramPosts.Count, allComments.Count, allHashtags.Count, allMentions.Count);
            
            return Result<InstagramData>.Ok(CreateInstagramData(instagramPosts, allComments, allHashtags, allMentions));
        }

        /// <summary>
        /// Cria um objeto InstagramData agregando posts, comentários, hashtags e menções.
        /// </summary>
        /// <param name="posts">Lista de posts.</param>
        /// <param name="comments">Lista de comentários.</param>
        /// <param name="hashtags">Lista de hashtags.</param>
        /// <param name="mentions">Lista de menções.</param>
        /// <returns>Objeto InstagramData agregado.</returns>
        private static InstagramData CreateInstagramData(IEnumerable<InstagramPost> posts, 
            IEnumerable<InstagramComment> comments, 
            IEnumerable<InstagramHashtag> hashtags, 
            IEnumerable<InstagramMention> mentions)
        {
            return new InstagramData
            {
                Posts = posts,
                Comments = comments,
                Hashtags = hashtags,
                Mentions = mentions
            };
        }

        /// <summary>
        /// Mapeia um ApifyInstagramPost para InstagramPost.
        /// </summary>
        /// <param name="apifyPost">Objeto ApifyInstagramPost a ser convertido.</param>
        /// <returns>Objeto InstagramPost correspondente.</returns>
        private static InstagramPost MapToInstagramPost(ApifyInstagramPost apifyPost)
        {
            return new InstagramPost
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
                Images = JsonSerializer.Serialize(apifyPost.Images),
                VideoUrl = apifyPost.VideoUrl,
                Alt = apifyPost.Alt ?? string.Empty,
                LikesCount = apifyPost.LikesCount,
                VideoViewCount = apifyPost.VideoViewCount,
                VideoPlayCount = apifyPost.VideoPlayCount,
                Timestamp = apifyPost.Timestamp,
                ChildPosts = JsonSerializer.Serialize(apifyPost.ChildPosts),
                OwnerFullName = apifyPost.OwnerFullName,
                OwnerUsername = apifyPost.OwnerUsername,
                OwnerId = apifyPost.OwnerId,
                ProductType = apifyPost.ProductType,
                VideoDuration = apifyPost.VideoDuration,
                IsSponsored = apifyPost.IsSponsored,
                TaggedUsers = JsonSerializer.Serialize(apifyPost.TaggedUsers),
                MusicInfo = apifyPost.MusicInfo != null ? JsonSerializer.Serialize(apifyPost.MusicInfo) : string.Empty,
                CoauthorProducers = JsonSerializer.Serialize(apifyPost.CoauthorProducers),
                IsCommentsDisabled = apifyPost.IsCommentsDisabled,
                InputUrl = apifyPost.InputUrl,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Mapeia comentários de um ApifyInstagramPost para uma lista de InstagramComment.
        /// </summary>
        /// <param name="apifyPost">Objeto ApifyInstagramPost contendo comentários.</param>
        /// <returns>Lista de comentários do post.</returns>
        private static IEnumerable<InstagramComment> MapToComments(ApifyInstagramPost apifyPost)
        {
            return apifyPost.LatestComments.Select(comment => new InstagramComment
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
                Replies = JsonSerializer.Serialize(comment.Replies),
                CreatedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Mapeia hashtags de um ApifyInstagramPost para uma lista de InstagramHashtag.
        /// </summary>
        /// <param name="apifyPost">Objeto ApifyInstagramPost contendo hashtags.</param>
        /// <returns>Lista de hashtags do post.</returns>
        private static IEnumerable<InstagramHashtag> MapToHashtags(ApifyInstagramPost apifyPost)
        {
            return apifyPost.Hashtags.Select(hashtag => new InstagramHashtag
            {
                PostId = apifyPost.Id,
                Hashtag = hashtag,
                CreatedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Mapeia menções de um ApifyInstagramPost para uma lista de InstagramMention.
        /// </summary>
        /// <param name="apifyPost">Objeto ApifyInstagramPost contendo menções.</param>
        /// <returns>Lista de menções do post.</returns>
        private static IEnumerable<InstagramMention> MapToMentions(ApifyInstagramPost apifyPost)
        {
            return apifyPost.TaggedUsers.Select(user => new InstagramMention
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
}
