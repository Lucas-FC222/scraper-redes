using Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Workers
{
    public class InstagramWorker : ScraperWorkerBase<string, IInstagramService>
    {
        public InstagramWorker(
            ILogger<InstagramWorker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger, serviceProvider, configuration, "InstagramWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
        { }

        protected override IEnumerable<string> GetTargets()
        {
            return _configuration.GetSection("InstagramScraper:Usernames").Get<string[]>() ?? Array.Empty<string>();
        }

        protected override async Task RunScraperAsync(IInstagramService service, string username, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper para perfil: {Username}", username);
            var runId = await service.RunScraperAsync(username, _configuration.GetValue<int>("InstagramScraper:Limit", 10));
            if (!string.IsNullOrEmpty(runId))
                _logger.LogInformation("Scraper do Instagram iniciado com sucesso para {Username}. RunId: {RunId}", username, runId);
            else
                _logger.LogError("Falha ao iniciar scraper do Instagram para {Username}", username);
        }
    }
}
