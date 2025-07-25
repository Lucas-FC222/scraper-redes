using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Instagram.Externals.Api.Client;
using Services.Features.Instagram.Models;
using Shared.Domain.Models;

namespace Services.Features.Instagram.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por executar o scraper do Instagram para um perfil específico,
    /// utilizando a API Apify e registrando logs do processo.
    /// </summary>
    public class RunScraperHandler : IRequestHandler<RunScraperRequest, Result<RunScraperResponse>>
    {
        /// <summary>
        /// Cliente para integração com a API Apify do Instagram.
        /// </summary>
        private readonly IApifyInstagramClient _apifyInstagramClient;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<RunScraperHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RunScraperHandler"/>.
        /// </summary>
        /// <param name="apifyInstagramClient">Cliente Apify para Instagram.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public RunScraperHandler(IApifyInstagramClient apifyInstagramClient, ILogger<RunScraperHandler> logger)
        {
            _apifyInstagramClient = apifyInstagramClient;
            _logger = logger;
        }

        /// <summary>
        /// Executa o scraper do Instagram para o perfil informado, retornando o identificador da execução.
        /// </summary>
        /// <param name="request">Requisição contendo o nome de usuário e o número máximo de posts.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da execução do scraper, incluindo o RunId ou detalhes do erro.</returns>
        public async Task<Result<RunScraperResponse>> Handle(RunScraperRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper do Instagram para: {Username}", request.Username);

            var result = await _apifyInstagramClient.RunInstagramScraperAsync(request.Username, request.Limit);

            if (!result.Success)
            {
                return Result<RunScraperResponse>.Fail(result.Error!);
            }

            if (string.IsNullOrEmpty(result.Data))
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Instagram scraper falhou",
                    Detail = "Falha ao iniciar scraper do Instagram",
                    Status = 400
                };

                _logger.LogError(problemDetails.Detail, problemDetails.Status);

                return Result<RunScraperResponse>.Fail(problemDetails);
            }

            _logger.LogInformation("Scraper do Instagram iniciado com sucesso. RunId: {RunId}", result.Data);

            return Result<RunScraperResponse>.Ok(new RunScraperResponse { RunId = result.Data });
        }
    }
}
