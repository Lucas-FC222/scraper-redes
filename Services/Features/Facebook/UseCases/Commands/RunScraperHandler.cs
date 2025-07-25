using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Externals.Api.Client;
using Services.Features.Facebook.Models;
using Shared.Domain.Models;

namespace Services.Features.Facebook.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por executar o scraper do Facebook para uma página específica,
    /// utilizando a API Apify e registrando logs do processo.
    /// </summary>
    public class RunScraperHandler : IRequestHandler<RunScraperRequest, Result<RunScraperResponse>>
    {
        /// <summary>
        /// Cliente para integração com a API Apify do Facebook.
        /// </summary>
        private readonly IApifyFacebookClient _apifyFacebookClient;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<RunScraperHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RunScraperHandler"/>.
        /// </summary>
        /// <param name="apifyFacebookClient">Cliente Apify para Facebook.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public RunScraperHandler(IApifyFacebookClient apifyFacebookClient, ILogger<RunScraperHandler> logger)
        {
            _apifyFacebookClient = apifyFacebookClient;
            _logger = logger;
        }

        /// <summary>
        /// Executa o scraper do Facebook para a página informada, retornando o identificador da execução.
        /// </summary>
        /// <param name="request">Requisição contendo a URL da página e o número máximo de posts.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da execução do scraper, incluindo o RunId ou detalhes do erro.</returns>
        public async Task<Result<RunScraperResponse>> Handle(RunScraperRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper do Facebook para página: {PageUrl}", request.PageUrl);

            var result = await _apifyFacebookClient.RunFacebookScraperAsync(request.PageUrl, request.MaxPosts);

            if (!result.Success)
            { 
                return Result<RunScraperResponse>.Fail(result.Error!);
            }

            if (string.IsNullOrEmpty(result.Data))
            {                
                var problemDetails = new ProblemDetails
                {
                    Title = "Facebook scraper falhou",
                    Detail = "Falha ao iniciar scraper do Facebook",
                    Status = 400
                };

                _logger.LogError(problemDetails.Detail, problemDetails.Status);

                return Result<RunScraperResponse>.Fail(problemDetails);
            }

            _logger.LogInformation("Scraper do Facebook iniciado com sucesso. RunId: {RunId}", result.Data);

            return Result<RunScraperResponse>.Ok(new RunScraperResponse { RunId = result.Data });
        }
    }
}
