using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Enums;
using System.Text.Json;

namespace Api.Controllers
{
    /// <summary>
    /// Controller responsável por receber e processar webhooks de plataformas externas como Facebook e Instagram.
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/webhook")]
    public class WebHookController : ApiControllerBase
    {
        private readonly string _webhookSecret = string.Empty;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="WebHookController"/>.
        /// </summary>
        /// <param name="config">Configuração da aplicação para obter o segredo do webhook.</param>
        /// <param name="mediator">Instância do MediatR injetada.</param>
        public WebHookController(IConfiguration config, IMediator mediator)
            : base(mediator)
        {
            _webhookSecret = config["Apify:WebhookSecret"] ?? "7YdK9fLm2Q";
        }

        /// <summary>
        /// Processa notificações de webhook do Facebook.
        /// </summary>
        /// <param name="payload">Payload recebido do webhook.</param>
        /// <returns>Resultado do processamento do webhook.</returns>
        [HttpPost("facebook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> ProcessWebhookInternalFacebookAsync([FromBody] JsonElement payload)
        {
            return ProcessWebhookInternalAsync(PostType.Facebook, payload, "Facebook");
        }

        /// <summary>
        /// Processa notificações de webhook do Instagram, validando o segredo.
        /// </summary>
        /// <param name="secret">Token secreto para validação do webhook.</param>
        /// <param name="payload">Payload recebido do webhook.</param>
        /// <returns>Resultado do processamento do webhook.</returns>
        [HttpPost("instagram")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> ProcessWebhookInternalInstagramAync([FromQuery] string secret, [FromBody] JsonElement payload)
        {
            if (secret != _webhookSecret)
            {
                return Task.FromResult<IActionResult>(UnauthorizedResult<object>("Token secreto inválido para webhook do Instagram"));
            }
            return ProcessWebhookInternalAsync(PostType.Instagram, payload, "Instagram");
        }

        /// <summary>
        /// Processa o payload do webhook de acordo com o tipo de post e plataforma.
        /// </summary>
        /// <param name="postType">Tipo de post (Facebook, Instagram, etc).</param>
        /// <param name="payload">Payload recebido do webhook.</param>
        /// <param name="platform">Nome da plataforma.</param>
        /// <returns>Resultado do processamento do webhook.</returns>
        private async Task<IActionResult> ProcessWebhookInternalAsync(PostType postType, JsonElement payload, string platform)
        {
            //if (!payload.TryGetProperty("eventType", out var eventTypeProp) || eventTypeProp.GetString() != "ACTOR.RUN.SUCCEEDED")
            //{
            //    _logger.LogInformation("Evento ignorado para {Platform} - não é ACTOR.RUN.SUCCEEDED", platform);
            //    return Success(new { Message = "Evento ignorado" });
            //}

            //if (!payload.TryGetProperty("resource", out var resourceProp) || !resourceProp.TryGetProperty("defaultDatasetId", out var datasetIdProp))
            //{
            //    _logger.LogError("Payload inválido para {Platform}: defaultDatasetId não encontrado", platform);
            //    return BadRequestResult<object>("Payload inválido: defaultDatasetId não encontrado");
            //}
            
            //var datasetId = datasetIdProp.GetString();
            //if (string.IsNullOrEmpty(datasetId) || datasetId.Length < 10)
            //{
            //    _logger.LogWarning("Dataset ID inválido ou de teste para {Platform}: {DatasetId}", platform, datasetId);
            //    return Success(new { Message = "Dataset ID inválido ou de teste" });
            //}

            var datasetId = payload.GetProperty("resource").GetProperty("defaultDatasetId").GetString()!;

            switch (postType)
            {
                case PostType.Facebook:
                    var facebookResult = await _mediator.Send(new Services.Features.Facebook.Models.ProcessDatasetRequest { DatasetId = datasetId });
                    if (!facebookResult.Success)
                    {
                        return BadRequest(facebookResult);
                    }
                    return Ok(facebookResult);

                case PostType.Instagram:
                    var instragramResult = await _mediator.Send(new Services.Features.Instagram.Models.ProcessDatasetRequest { DatasetId = datasetId });
                    if (!instragramResult.Success)
                    {
                        return BadRequest(instragramResult);
                    }
                    return Ok(instragramResult);

                // Adicione outros casos se necessário
                default:
                    throw new NotSupportedException($"PostType '{postType}' não é suportado.");
            }
        }
    }
}
