using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebHookController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebHookController> _logger;
        private readonly string _webhookSecret;

        public WebHookController(IServiceProvider serviceProvider, ILogger<WebHookController> logger, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _webhookSecret = config["Apify:WebhookSecret"] ?? "7YdK9fLm2Q";
        }

        [HttpPost("facebook")]
        public Task<IActionResult> Facebook([FromBody] JsonElement payload)
        {
            return ProcessWebhookInternalAsync<IFacebookService>(payload, "Facebook");
        }

        [HttpPost("instagram")]
        public Task<IActionResult> Instagram([FromQuery] string secret, [FromBody] JsonElement payload)
        {
            if (secret != _webhookSecret)
            {
                _logger.LogWarning("Secret inválido recebido para webhook do Instagram");
                return Task.FromResult<IActionResult>(Unauthorized());
            }
            return ProcessWebhookInternalAsync<IInstagramService>(payload, "Instagram");
        }

        private async Task<IActionResult> ProcessWebhookInternalAsync<TService>(JsonElement payload, string platform)
            where TService : class
        {
            _logger.LogInformation("Processando webhook para a plataforma: {Platform}", platform);
            
            try
            {
                if (!payload.TryGetProperty("eventType", out var eventTypeProp) || eventTypeProp.GetString() != "ACTOR.RUN.SUCCEEDED")
                {
                    _logger.LogInformation("Evento ignorado para {Platform} - não é ACTOR.RUN.SUCCEEDED", platform);
                    return Ok(new { Message = "Evento ignorado" });
                }

                if (!payload.TryGetProperty("resource", out var resourceProp) || !resourceProp.TryGetProperty("defaultDatasetId", out var datasetIdProp))
                {
                    _logger.LogError("Payload inválido para {Platform}: defaultDatasetId não encontrado", platform);
                    return BadRequest("Payload inválido: defaultDatasetId não encontrado");
                }
                
                var datasetId = datasetIdProp.GetString();
                if (string.IsNullOrEmpty(datasetId) || datasetId.Length < 10)
                {
                    _logger.LogWarning("Dataset ID inválido ou de teste para {Platform}: {DatasetId}", platform, datasetId);
                    return Ok(new { Message = "Dataset ID inválido ou de teste" });
                }

                _logger.LogInformation("Dataset ID extraído para {Platform}: {DatasetId}", platform, datasetId);

                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();

                if (service is IFacebookService facebookService)
                {
                    await facebookService.ProcessDatasetAsync(datasetId);
                }
                else if (service is IInstagramService instagramService)
                {
                    await instagramService.ProcessDatasetAsync(datasetId);
                }

                _logger.LogInformation("Processamento do dataset para {Platform} concluído com sucesso", platform);
                return Ok(new { message = $"Webhook para {platform} processado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook para {Platform}", platform);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
