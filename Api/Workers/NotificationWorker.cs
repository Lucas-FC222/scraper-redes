using Core.Services;

namespace Api.Workers
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly TimeSpan _interval;

        public NotificationWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationWorker> logger,
            IConfiguration cfg)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _interval = TimeSpan.FromSeconds(cfg.GetValue("Notifications:IntervalSeconds", 60));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationWorker starting, interval {sec}s", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessorService>();

                try
                {
                    await processor.RunAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in notification processor");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
