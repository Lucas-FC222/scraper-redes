using MediatR;
using Services.Features.Instagram.Models;

namespace Api.Workers
{
    /// <summary>
    /// Worker respons�vel por executar o scraping de perfis do Instagram conforme configura��o.
    /// </summary>
    /// <param name="logger">Logger para registro de eventos.</param>
    /// <param name="serviceProvider">Provider de servi�os para DI.</param>
    /// <param name="configuration">Configura��o da aplica��o.</param>
    public class InstagramWorker(
        ILogger<InstagramWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration) : ScraperWorkerBase<string, IMediator>(logger, serviceProvider, configuration, "InstagramWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
    {
        /// <summary>
        /// Obt�m a lista de usernames do Instagram a serem processados a partir da configura��o.
        /// </summary>
        /// <returns>Lista de usernames do Instagram.</returns>
        protected override IEnumerable<string> GetTargets()
        {
            return _configuration.GetSection("InstagramScraper:Usernames").Get<string[]>() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Executa o scraper para um perfil espec�fico do Instagram utilizando o MediatR.
        /// </summary>
        /// <param name="mediator">Inst�ncia do MediatR para envio de comandos.</param>
        /// <param name="username">Username do perfil do Instagram.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        protected override async Task RunScraperAsync(IMediator mediator, string username, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper para perfil: {Username}", username);

            await mediator.Send(new RunScraperRequest() { Username = username, Limit = _configuration.GetValue("InstagramScraper:Limit", 10) });
        }
    }
}
