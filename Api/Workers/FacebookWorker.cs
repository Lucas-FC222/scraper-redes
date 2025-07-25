using MediatR;
using Services.Features.Facebook.Models;

namespace Api.Workers
{
    /// <summary>
    /// Worker responsável por executar o scraping de páginas do Facebook conforme configuração.
    /// Herda de <see cref="ScraperWorkerBase{FacebookPageConfig, IMediator}"/> para gerenciar o ciclo de scraping.
    /// </summary>
    /// <param name="logger">Logger para registro de eventos.</param>
    /// <param name="serviceProvider">Provider de serviços para DI.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    public class FacebookWorker(
        ILogger<FacebookWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration) : ScraperWorkerBase<FacebookPageConfig, IMediator>(logger, serviceProvider, configuration, "FacebookWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
    {
        /// <summary>
        /// Obtém a lista de páginas do Facebook a serem processadas a partir da configuração.
        /// </summary>
        /// <returns>Lista de configurações de páginas do Facebook.</returns>
        protected override IEnumerable<FacebookPageConfig> GetTargets()
        {
            var config = _configuration.GetSection("FacebookScraper:Pages").Get<List<FacebookPageConfig>>();
            return config ?? Enumerable.Empty<FacebookPageConfig>();
        }

        /// <summary>
        /// Executa o scraper para uma página específica do Facebook utilizando o MediatR.
        /// </summary>
        /// <param name="mediator">Instância do MediatR para envio de comandos.</param>
        /// <param name="page">Configuração da página do Facebook.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        protected override async Task RunScraperAsync(IMediator mediator, FacebookPageConfig page, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper para página: {PageName} ({PageUrl})", page.Name, page.PageUrl);

            await mediator.Send(new RunScraperRequest() { PageUrl = page.PageUrl, MaxPosts = _configuration.GetValue("FacebookScraper:MaxPosts", 10)});
        }
    }
}
