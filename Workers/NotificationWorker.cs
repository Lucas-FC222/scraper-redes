using Shared.Services;
using Services;

namespace Workers
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
            _interval = TimeSpan.FromSeconds(cfg.GetValue<int>("Notifications:IntervalSeconds", 60));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationWorker starting, interval {sec}s", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessorService>();

                await processor.RunAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
