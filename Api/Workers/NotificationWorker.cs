namespace Api.Workers
{
    /// <summary>
    /// Worker responsável por processar notificações periodicamente utilizando um serviço de processamento de notificações.
    /// Executa em background com intervalo configurável.
    /// </summary>
    public class NotificationWorker : BackgroundService
    {
        /// <summary>
        /// Fábrica de escopos para resolução de dependências.
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        /// Logger para registro de eventos e erros.
        /// </summary>
        private readonly ILogger<NotificationWorker> _logger;
        /// <summary>
        /// Intervalo entre execuções do processamento de notificações.
        /// </summary>
        private readonly TimeSpan _interval;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="NotificationWorker"/>.
        /// </summary>
        /// <param name="scopeFactory">Fábrica de escopos para DI.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="cfg">Configuração da aplicação.</param>
        public NotificationWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationWorker> logger,
            IConfiguration cfg)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _interval = TimeSpan.FromSeconds(cfg.GetValue("Notifications:IntervalSeconds", 60));
        }

        /// <summary>
        /// Executa o processamento de notificações em intervalos definidos enquanto o serviço não for cancelado.
        /// </summary>
        /// <param name="stoppingToken">Token de cancelamento.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationWorker starting, interval {sec}s", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                //using var scope = _scopeFactory.CreateScope();
                //var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessorService>();

                //try
                //{
                //    await processor.RunAsync(stoppingToken);
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, "Error in notification processor");
                //}

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
