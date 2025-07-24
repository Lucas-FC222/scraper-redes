using Api.Models;
using Shared.Services;

namespace Api.Workers
{
    /// <summary>
    /// Worker responsável por executar o scraper de páginas do Facebook periodicamente.
    /// </summary>
    public class FacebookWorker : ScraperWorkerBase<FacebookPageConfig, IFacebookService>
    {
        /// <summary>
        /// Inicializa o FacebookWorker.
        /// </summary>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="serviceProvider">Provider de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public FacebookWorker(
            ILogger<FacebookWorker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger, serviceProvider, configuration, "FacebookWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
        { }

        /// <summary>
        /// Obtém as páginas do Facebook configuradas para scraping.
        /// </summary>
        /// <returns>Lista de configurações de páginas do Facebook.</returns>
        protected override IEnumerable<FacebookPageConfig> GetTargets()
        {
            var config = _configuration.GetSection("FacebookScraper:Pages").Get<List<FacebookPageConfig>>();
            return config ?? Enumerable.Empty<FacebookPageConfig>();
        }

        /// <summary>
        /// Executa o scraper para uma página específica do Facebook.
        /// </summary>
        /// <param name="service">Serviço de scraping do Facebook.</param>
        /// <param name="page">Configuração da página alvo.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        protected override async Task RunScraperAsync(IFacebookService service, FacebookPageConfig page, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper para página: {PageName} ({PageUrl})", page.Name, page.PageUrl);
            var runId = await service.RunScraperAsync(page.PageUrl, _configuration.GetValue("FacebookScraper:MaxPosts", 10));
            if (!string.IsNullOrEmpty(runId))
                _logger.LogInformation("Scraper do Facebook iniciado com sucesso para {PageName}. RunId: {RunId}", page.Name, runId);
            else
                _logger.LogError("Falha ao iniciar scraper do Facebook para {PageName}", page.Name);
        }
    }
}
