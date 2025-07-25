using Microsoft.Extensions.Configuration;
using Shared.Domain.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services.Features.Posts.Externals.Client
{
    /// <summary>
    /// Cliente responsável por classificar o conteúdo de posts utilizando a API Groq.
    /// </summary>
    public class GroqApiClient : IGroqApiClient
    {
        /// <summary>
        /// Cliente HTTP utilizado para requisições à API Groq.
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// Chave de API utilizada para autenticação na Groq.
        /// </summary>
        private readonly string _apiKey = string.Empty;
        /// <summary>
        /// URI da requisição para a API Groq.
        /// </summary>
        private readonly string _groqRequestUri;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GroqApiClient"/>.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP injetado.</param>
        /// <param name="configuration">Configuração da aplicação para obter chave e URI da API Groq.</param>
        public GroqApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Groq:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API key cannot be null.");
            _groqRequestUri = configuration["Groq:RequestUri"] ?? throw new ArgumentNullException(nameof(configuration), "Groq request URI cannot be null.");
        }

        /// <summary>
        /// Classifica o texto de um post em um tema específico utilizando a API Groq.
        /// </summary>
        /// <param name="text">Texto do post a ser classificado.</param>
        /// <returns>Resultado contendo o tema classificado ou erro.</returns>
        public async Task<Result<string>> ClassifyPostAsync(string text)
        {
            var prompt = $"Classifique o tema deste post em: esporte, política, tecnologia, entretenimento, outros. Responda apenas com o tema. Post: {text}";

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            HttpRequestMessage groqRequest = new HttpRequestMessage(HttpMethod.Post, _groqRequestUri);
            groqRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            groqRequest.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(groqRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Result<string>.Ok(result?.Trim() ?? "outros");
        }
    }
}
