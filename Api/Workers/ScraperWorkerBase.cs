namespace Api.Workers
{
    /// <summary>
    /// Classe base abstrata para workers de scraping de redes sociais.
    /// Gerencia o ciclo de execução, escopo de serviços e controle de alvos.
    /// </summary>
    /// <typeparam name="TTarget">Tipo do alvo de scraping (ex: página, username).</typeparam>
    /// <typeparam name="TService">Tipo do serviço de scraping utilizado.</typeparam>
    public abstract class ScraperWorkerBase<TTarget, TService> : BackgroundService
        where TService : class
    {
        /// <summary>
        /// Logger para registro de eventos.
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// Provider de serviços para escopo de dependências.
        /// </summary>
        protected readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Configuração da aplicação.
        /// </summary>
        protected readonly IConfiguration _configuration;
        private readonly string _workerName;
        private readonly int _delaySeconds;
        private readonly int _delayBetweenTargetsSeconds;

        /// <summary>
        /// Inicializa o worker base de scraping.
        /// </summary>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="serviceProvider">Provider de serviços para DI.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        /// <param name="workerName">Nome do worker.</param>
        /// <param name="delaySeconds">Intervalo entre execuções completas.</param>
        /// <param name="delayBetweenTargetsSeconds">Intervalo entre execuções para cada alvo.</param>
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

        /// <summary>
        /// Obtém a lista de alvos para scraping.
        /// </summary>
        /// <returns>Lista de alvos do tipo <typeparamref name="TTarget"/>.</returns>
        protected abstract IEnumerable<TTarget> GetTargets();
        /// <summary>
        /// Executa o scraper para um alvo específico.
        /// </summary>
        /// <param name="service">Serviço de scraping.</param>
        /// <param name="target">Alvo do scraping.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        protected abstract Task RunScraperAsync(TService service, TTarget target, CancellationToken cancellationToken);

        /// <summary>
        /// Executa o ciclo principal do worker, processando todos os alvos em intervalos definidos.
        /// </summary>
        /// <param name="stoppingToken">Token de cancelamento.</param>
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