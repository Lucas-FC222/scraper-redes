using Api.Models;
using Core.Services;

namespace Api.Workers
{
    public class FacebookWorker : ScraperWorkerBase<FacebookPageConfig, IFacebookService>
    {
        public FacebookWorker(
            ILogger<FacebookWorker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger, serviceProvider, configuration, "FacebookWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
        { }

        protected override IEnumerable<FacebookPageConfig> GetTargets()
        {
            var config = _configuration.GetSection("FacebookScraper:Pages").Get<List<FacebookPageConfig>>();
            return config ?? Enumerable.Empty<FacebookPageConfig>();
        }

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
