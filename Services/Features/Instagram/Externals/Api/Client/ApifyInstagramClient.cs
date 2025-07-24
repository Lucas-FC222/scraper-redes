using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Features.Instagram.Models;
using Shared.Domain.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Services.Features.Instagram.Externals.Api.Client
{
    public class ApifyInstagramClient : IApifyInstagramClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApifyInstagramClient> _logger;
        private readonly IOptions<Models.ApifySettings> _settings;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

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
        /// <returns>RunId do scraper ou string vazia se falhar.</returns>
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
        /// <returns>Dados do Instagram processados ou null.</returns>
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
        private static InstagramPost MapToInstagramPost(ApifyInstagramPost apifyPost)
        {
            return JsonSerializer.Deserialize<InstagramPost>(JsonSerializer.Serialize(apifyPost)) ?? new InstagramPost();
        }

        /// <summary>
        /// Mapeia comentários de um ApifyInstagramPost para uma lista de InstagramComment.
        /// </summary>
        private static IEnumerable<InstagramComment> MapToComments(ApifyInstagramPost apifyPost)
        {
            return apifyPost.LatestComments.Select(comment => JsonSerializer.Deserialize<InstagramComment>(JsonSerializer.Serialize(comment)))!;
        }

        /// <summary>
        /// Mapeia hashtags de um ApifyInstagramPost para uma lista de InstagramHashtag.
        /// </summary>
        private static IEnumerable<InstagramHashtag> MapToHashtags(ApifyInstagramPost apifyPost)
        {
            return JsonSerializer.Deserialize<IEnumerable<InstagramHashtag>>(JsonSerializer.Serialize(apifyPost)) ?? [];
        }

        /// <summary>
        /// Mapeia menções de um ApifyInstagramPost para uma lista de InstagramMention.
        /// </summary>
        private static IEnumerable<InstagramMention> MapToMentions(ApifyInstagramPost apifyPost)
        {
            return JsonSerializer.Deserialize<IEnumerable<InstagramMention>>(JsonSerializer.Serialize(apifyPost)) ?? [];
        }
    }
}
