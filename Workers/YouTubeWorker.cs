
using Services;


namespace Workers
{
    public class YouTubeWorker : ScraperWorkerBase<string, IYouTubeAnalyzerService>
    {
        public YouTubeWorker(
            ILogger<YouTubeWorker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger, serviceProvider, configuration, "YouTubeWorker",
                  delaySeconds: configuration.GetValue<int>("YouTubeScraper:IntervalSeconds", 600),
                  delayBetweenTargetsSeconds: configuration.GetValue<int>("YouTubeScraper:DelayBetweenChannelsSeconds", 60))
        { }

        protected override IEnumerable<string> GetTargets()
        {
            return _configuration.GetSection("YouTubeScraper:ChannelIds").Get<string[]>()
                   ?? Array.Empty<string>();
        }

        protected override async Task RunScraperAsync(IYouTubeAnalyzerService service,
                                                     string channelId,
                                                     CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando scraping do canal {ChannelId}", channelId);
            try
            {
                await service.AnalyzeChannelAsync(channelId, null);
                _logger.LogInformation("Scraping concluído para canal {ChannelId}", channelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no scraping do canal {ChannelId}", channelId);
            }
        }
    }
}