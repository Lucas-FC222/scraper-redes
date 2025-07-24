using Shared.Services;

namespace Api.Workers
{
    /// <summary>
    /// Worker respons�vel por executar o scraper de perfis do Instagram periodicamente.
    /// </summary>
    public class InstagramWorker : ScraperWorkerBase<string, IInstagramService>
    {
        /// <summary>
        /// Inicializa o InstagramWorker.
        /// </summary>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="serviceProvider">Provider de servi�os para DI.</param>
        /// <param name="configuration">Configura��o da aplica��o.</param>
        public InstagramWorker(
            ILogger<InstagramWorker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger, serviceProvider, configuration, "InstagramWorker", delaySeconds: 300, delayBetweenTargetsSeconds: 30)
        { }

        /// <summary>
        /// Obt�m os usernames do Instagram configurados para scraping.
        /// </summary>
        /// <returns>Lista de usernames do Instagram.</returns>
        protected override IEnumerable<string> GetTargets()
        {
            return _configuration.GetSection("InstagramScraper:Usernames").Get<string[]>() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Executa o scraper para um perfil espec�fico do Instagram.
        /// </summary>
        /// <param name="service">Servi�o de scraping do Instagram.</param>
        /// <param name="username">Username do perfil alvo.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        protected override async Task RunScraperAsync(IInstagramService service, string username, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando scraper para perfil: {Username}", username);
            var runId = await service.RunScraperAsync(username, _configuration.GetValue("InstagramScraper:Limit", 10));
            if (!string.IsNullOrEmpty(runId))
                _logger.LogInformation("Scraper do Instagram iniciado com sucesso para {Username}. RunId: {RunId}", username, runId);
            else
                _logger.LogError("Falha ao iniciar scraper do Instagram para {Username}", username);
        }
    }
}
