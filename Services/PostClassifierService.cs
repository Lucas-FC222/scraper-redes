using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Core.Services;

namespace Services
{
  public class PostClassifierService : IPostClassifierService
  {
        private readonly ILogger<PostClassifierService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public PostClassifierService(ILogger<PostClassifierService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiKey = configuration["Groq:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API key cannot be null.");
        }

        public async Task<string> ClassifyPostAsync(string text)
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

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result?.Trim() ?? "outros";
        }
  }
}