using Infra;
using Infra.Data;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class FacebookService : IFacebookService
    {
        private readonly IApiFyService _apiFyService;
        private readonly IFacebookRepository _facebookRepository;
        private readonly ILogger<FacebookService> _logger;
        private readonly IPostClassifierService _postClassifierService;

        public FacebookService(IApiFyService apiFyService, IFacebookRepository facebookRepository, ILogger<FacebookService> logger, IPostClassifierService postClassifierService)
        {
            _apiFyService = apiFyService;
            _facebookRepository = facebookRepository;
            _logger = logger;
            _postClassifierService = postClassifierService;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Iniciando serviço do Facebook");
                _logger.LogInformation("Serviço do Facebook executado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar serviço do Facebook");
            }
        }

        public async Task<string?> RunScraperAsync(string pageUrl, int maxPosts)
        {
            try
            {
                _logger.LogInformation("Executando scraper do Facebook para página: {PageUrl}", pageUrl);
                var runId = await _apiFyService.RunFacebookScraperAsync(pageUrl, maxPosts);
                if (string.IsNullOrEmpty(runId))
                {
                    _logger.LogError("Falha ao iniciar scraper do Facebook");
                    return null;
                }
                _logger.LogInformation("Scraper do Facebook iniciado com sucesso. RunId: {RunId}", runId);
                return runId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar scraper do Facebook");
                return null;
            }
        }

        public async Task<IEnumerable<Core.FacebookPost>> ProcessDatasetAsync(string datasetId)
        {
            try
            {
                _logger.LogInformation("Processando dataset do Facebook: {DatasetId}", datasetId);

                var facebookData = await _apiFyService.ProcessFacebookDatasetAsync(datasetId);

                if (facebookData == null || !facebookData.Posts.Any())
                {
                    _logger.LogWarning("Nenhum post encontrado no dataset do Facebook: {DatasetId}", datasetId);
                    return Array.Empty<Core.FacebookPost>();
                }

                foreach (var post in facebookData.Posts)
                {
                    // Preencher o PageUrl
                    // post.PageUrl = pageUrl;
                }

                // Filtrar duplicatas de Id
                var postsUnicos = facebookData.Posts
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();

                foreach (var post in postsUnicos)
                {
                    post.Topic = await _postClassifierService.ClassifyPostAsync(post.Message ?? "");
                }

                await _facebookRepository.SavePostsAsync(postsUnicos);
                _logger.LogInformation("{Count} posts do Facebook salvos com sucesso", postsUnicos.Count);

                return postsUnicos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar dataset do Facebook: {DatasetId}", datasetId);
                return Array.Empty<Core.FacebookPost>();
            }
        }

        public async Task<IEnumerable<Core.FacebookPost>> GetAllPostsAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todos os posts do Facebook");
                return await _facebookRepository.GetAllPostsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts do Facebook");
                return Enumerable.Empty<Core.FacebookPost>();
            }
        }

        public async Task<Core.FacebookPost?> GetPostByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Buscando post do Facebook com ID: {Id}", id);
                return await _facebookRepository.GetPostByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post do Facebook com ID: {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Core.FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            return await _facebookRepository.SearchPostsByKeywordsAsync(keywords);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}