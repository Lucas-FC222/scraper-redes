using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Core.Services;

namespace Api.Controllers
{
    /// <summary>
    /// Controller para receber e processar webhooks de serviços externos
    /// </summary>
    [ApiController]
    [Route("api/webhook")]
    public class WebHookController : ApiControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebHookController> _logger;
        private readonly string _webhookSecret;

        /// <summary>
        /// Inicializa uma nova instância do controlador de webhooks
        /// </summary>
        /// <param name="serviceProvider">Provedor de serviços para resolução de dependências</param>
        /// <param name="logger">Logger para registro de eventos</param>
        /// <param name="config">Configurações da aplicação</param>
        public WebHookController(IServiceProvider serviceProvider, ILogger<WebHookController> logger, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _webhookSecret = config["Apify:WebhookSecret"] ?? "7YdK9fLm2Q";
        }

        /// <summary>
        /// Recebe e processa webhooks do Facebook enviados por serviços externos.
        /// </summary>
        /// <param name="payload">Payload do webhook recebido.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Webhook processado com sucesso ou ignorado.</para>
        /// <para><b>400 BadRequest</b>: Payload inválido.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPost("facebook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> Facebook([FromBody] JsonElement payload)
        {
            return ProcessWebhookInternalAsync<IFacebookService>(payload, "Facebook");
        }

        /// <summary>
        /// Recebe e processa webhooks do Instagram enviados por serviços externos.
        /// </summary>
        /// <param name="secret">Token secreto para validação do webhook.</param>
        /// <param name="payload">Payload do webhook recebido.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Webhook processado com sucesso ou ignorado.</para>
        /// <para><b>401 Unauthorized</b>: Token secreto inválido.</para>
        /// <para><b>400 BadRequest</b>: Payload inválido.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPost("instagram")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> Instagram([FromQuery] string secret, [FromBody] JsonElement payload)
        {
            if (secret != _webhookSecret)
            {
                _logger.LogWarning("Secret inválido recebido para webhook do Instagram");
                return Task.FromResult<IActionResult>(UnauthorizedResult<object>("Token secreto inválido para webhook do Instagram"));
            }
            return ProcessWebhookInternalAsync<IInstagramService>(payload, "Instagram");
        }

        /// <summary>
        /// Processa internamente os webhooks recebidos do Apify para Facebook e Instagram
        /// </summary>
        /// <typeparam name="TService">Tipo do serviço a ser usado para processar o dataset</typeparam>
        /// <param name="payload">Payload JSON recebido do webhook</param>
        /// <param name="platform">Nome da plataforma (Facebook ou Instagram)</param>
        /// <returns>Resultado da operação para retorno ao cliente</returns>
        private async Task<IActionResult> ProcessWebhookInternalAsync<TService>(JsonElement payload, string platform)
            where TService : class
        {
            _logger.LogInformation("Processando webhook para a plataforma: {Platform}", platform);
            
            if (!payload.TryGetProperty("eventType", out var eventTypeProp) || eventTypeProp.GetString() != "ACTOR.RUN.SUCCEEDED")
            {
                _logger.LogInformation("Evento ignorado para {Platform} - não é ACTOR.RUN.SUCCEEDED", platform);
                return Success(new { Message = "Evento ignorado" });
            }

            if (!payload.TryGetProperty("resource", out var resourceProp) || !resourceProp.TryGetProperty("defaultDatasetId", out var datasetIdProp))
            {
                _logger.LogError("Payload inválido para {Platform}: defaultDatasetId não encontrado", platform);
                return BadRequestResult<object>("Payload inválido: defaultDatasetId não encontrado");
            }
            
            var datasetId = datasetIdProp.GetString();
            if (string.IsNullOrEmpty(datasetId) || datasetId.Length < 10)
            {
                _logger.LogWarning("Dataset ID inválido ou de teste para {Platform}: {DatasetId}", platform, datasetId);
                return Success(new { Message = "Dataset ID inválido ou de teste" });
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
            return Success(new { message = $"Webhook para {platform} processado com sucesso" });
        }
    }
}
