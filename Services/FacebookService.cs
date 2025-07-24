using Core.Models;
using Core.Repositories;
using Core.Services;
using Infra.Externals.ApiFy.Interfaces;
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

        public async Task<string?> RunScraperAsync(string pageUrl, int maxPosts)
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

        public async Task<IEnumerable<FacebookPost>> ProcessDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Processando dataset do Facebook: {DatasetId}", datasetId);

            var facebookData = await _apiFyService.ProcessFacebookDatasetAsync(datasetId);

            if (facebookData == null || !facebookData.Posts.Any())
            {
                _logger.LogWarning("Nenhum post encontrado no dataset do Facebook: {DatasetId}", datasetId);
                return Array.Empty<FacebookPost>();
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

        public async Task<IEnumerable<FacebookPost>> GetAllPostsAsync()
        {
            _logger.LogInformation("Buscando todos os posts do Facebook");
            return await _facebookRepository.GetAllPostsAsync();
        }

        public async Task<FacebookPost?> GetPostByIdAsync(string id)
        {
            _logger.LogInformation("Buscando post do Facebook com ID: {Id}", id);
            return await _facebookRepository.GetPostByIdAsync(id);
        }

        public async Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            return await _facebookRepository.SearchPostsByKeywordsAsync(keywords);
        }
    }
}