namespace Api.Workers
{
    public abstract class ScraperWorkerBase<TTarget, TService> : BackgroundService
        where TService : class
    {
        protected readonly ILogger _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;
        private readonly string _workerName;
        private readonly int _delaySeconds;
        private readonly int _delayBetweenTargetsSeconds;

        protected ScraperWorkerBase(
            ILogger logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            string workerName,
            int delaySeconds = 300,
            int delayBetweenTargetsSeconds = 30)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _workerName = workerName;
            _delaySeconds = delaySeconds;
            _delayBetweenTargetsSeconds = delayBetweenTargetsSeconds;
        }

        protected abstract IEnumerable<TTarget> GetTargets();
        protected abstract Task RunScraperAsync(TService service, TTarget target, CancellationToken cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Worker} iniciado em: {Time}", _workerName, DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var targets = GetTargets().ToList();
                    if (!targets.Any())
                    {
                        _logger.LogWarning("Nenhum alvo configurado para {Worker}", _workerName);
                    }
                    else
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<TService>();

                        foreach (var target in targets)
                        {
                            try
                            {
                                await RunScraperAsync(service, target, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Erro ao executar scraper para alvo {Target} em {Worker}", target, _workerName);
                            }

                            await Task.Delay(TimeSpan.FromSeconds(_delayBetweenTargetsSeconds), stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante execução do {Worker}", _workerName);
                }

                await Task.Delay(TimeSpan.FromSeconds(_delaySeconds), stoppingToken);
            }
        }
    }
} 