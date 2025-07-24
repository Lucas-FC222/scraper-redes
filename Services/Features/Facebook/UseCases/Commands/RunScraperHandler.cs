using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Externals.Api.Client;
using Services.Features.Facebook.Models;
using Shared.Domain.Models;

namespace Services.Features.Facebook.UseCases.Commands
{
    public class RunScraperHandler : IRequestHandler<RunScraperRequest, Result<RunScraperResponse>>
    {
        private readonly IApifyFacebookClient _apifyFacebookClient;
        private readonly ILogger<RunScraperHandler> _logger;

        public RunScraperHandler(IApifyFacebookClient apifyFacebookClient, ILogger<RunScraperHandler> logger)
        {
            _apifyFacebookClient = apifyFacebookClient;
            _logger = logger;
        }

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
                    Status = 500
                };

                _logger.LogError(problemDetails.Detail, problemDetails.Status);

                return Result<RunScraperResponse>.Fail(problemDetails);
            }

            _logger.LogInformation("Scraper do Facebook iniciado com sucesso. RunId: {RunId}", result.Data);

            return Result<RunScraperResponse>.Ok(new RunScraperResponse { RunId = result.Data });
        }
    }
}
