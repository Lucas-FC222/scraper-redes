using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Features.Facebook.Models;
using Shared.Domain.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Services.Features.Facebook.Externals.Api.Client
{
    public class ApifyFacebookClient : IApifyFacebookClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApifyFacebookClient> _logger;
        private readonly IOptions<Models.ApifySettings> _settings;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        public ApifyFacebookClient(HttpClient httpClient, ILogger<ApifyFacebookClient> logger, IOptions<Models.ApifySettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        /// Inicia o scraper do Facebook para uma página específica.
        /// </summary>
        /// <param name="pageUrl">URL da página do Facebook.</param>
        /// <param name="maxPosts">Número máximo de posts a coletar.</param>
        /// <returns>RunId do scraper ou string vazia se falhar.</returns>
        public async Task<Result<string>> RunFacebookScraperAsync(string pageUrl, int maxPosts)
        {
            _logger.LogInformation("Iniciando scraper do Facebook para página: {PageUrl}, limite: {MaxPosts}", pageUrl, maxPosts);

            var input = new { query = pageUrl, max_posts = maxPosts, search_type = "posts" };
            var startUrl = $"https://api.apify.com/v2/acts/l6CUZt8H0214D3I0N/runs?token={_settings.Value.ApiToken}";
            var startResp = await _httpClient.PostAsJsonAsync(startUrl, input);

            if (!startResp.IsSuccessStatusCode)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Facebook scraper falhou",
                    Detail = $"Erro ao iniciar scraper do Facebook. Status: {startResp.StatusCode}",
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
                    Title = "Facebook scraper falhou",
                    Detail = $"RunId não pode ser nulo ou vazio. Status: {startResp.StatusCode}",
                    Status = (int)startResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, startResp.StatusCode);

                return Result<string>.Fail(problemDetails);
            }

            _logger.LogInformation("Scraper do Facebook iniciado com sucesso. RunId: {RunId}", runId);
            
            return Result<string>.Ok(runId);
        }

        /// <summary>
        /// Processa o dataset do Facebook retornado pela Apify.
        /// </summary>
        /// <param name="datasetId">ID do dataset.</param>
        /// <returns>Dados do Facebook processados ou null.</returns>
        public async Task<Result<FacebookData>> ProcessFacebookDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Iniciando processamento do dataset do Facebook: {DatasetId}", datasetId);

            var itemsUrl = $"https://api.apify.com/v2/datasets/{datasetId}/items?token={_settings.Value.ApiToken}";
            _logger.LogInformation("Fazendo requisição para: {Url}", itemsUrl.Replace(_settings.Value.ApiToken, "***"));
            
            var itemsResp = await _httpClient.GetAsync(itemsUrl);  
            _logger.LogInformation("Resposta da API Apify - Status: {StatusCode}", itemsResp.StatusCode);
            
            if (!itemsResp.IsSuccessStatusCode)
            {
                var errorContent = await itemsResp.Content.ReadAsStringAsync();

                var problemDetails = new ProblemDetails
                {
                    Title = "Facebook scraper falhou",
                    Detail = $"Erro na requisição para Apify. Status: {itemsResp.StatusCode}, Content: {errorContent}",
                    Status = (int)itemsResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, itemsResp.StatusCode);

                return Result<FacebookData>.Fail(problemDetails);
            }

            var json = await itemsResp.Content.ReadAsStringAsync();
            _logger.LogInformation("JSON recebido da API Apify. Tamanho: {Size} caracteres", json.Length);

            var preview = json.Length > 500 ? string.Concat(json.AsSpan(0, 500), "...") : json;
            _logger.LogInformation("Preview do JSON: {Preview}", preview);

            var apifyItems = JsonSerializer.Deserialize<IEnumerable<ApifyFacebookPost>>(json, _jsonSerializerOptions);
            if (apifyItems == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Facebook scraper falhou",
                    Detail = "Falha na desserialização do JSON para ApifyFacebookPost",
                    Status = (int)itemsResp.StatusCode
                };

                _logger.LogError(problemDetails.Detail, itemsResp.StatusCode);

                return Result<FacebookData>.Fail(problemDetails);
            }

            _logger.LogInformation("Desserialização bem-sucedida. {Count} itens encontrados", apifyItems.Count());
            var facebookPosts = apifyItems.Select(MapToFacebookPost).ToList();

            _logger.LogInformation("Mapeamento concluído. {PostsCount} posts do Facebook mapeados", facebookPosts.Count);
            return  Result<FacebookData>.Ok(new FacebookData { Posts = facebookPosts });
        }

        // Métodos privados
        /// <summary>
        /// Mapeia um ApifyFacebookPost para FacebookPost.
        /// </summary>
        private static FacebookPost MapToFacebookPost(ApifyFacebookPost apifyPost)
        {
            return JsonSerializer.Deserialize<FacebookPost>(JsonSerializer.Serialize(apifyPost)) ?? new FacebookPost();
        }
    }
}
