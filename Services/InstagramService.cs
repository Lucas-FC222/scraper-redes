using Core.Models;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class InstagramService : IInstagramService
    {
        private readonly ICrowlerService _crowlerService;
        private readonly IInstagramRepository _instagramRepository;
        private readonly ILogger<InstagramService> _logger;
        private readonly IPostClassifierService _postClassifierService;

        public InstagramService(ICrowlerService crowlerService, IInstagramRepository instagramRepository, ILogger<InstagramService> logger, IPostClassifierService postClassifierService)
        {
            _crowlerService = crowlerService;
            _instagramRepository = instagramRepository;
            _logger = logger;
            _postClassifierService = postClassifierService;

        }

        public async Task<string?> RunScraperAsync(string username, int limit)
        {
            _logger.LogInformation("Executando scraper do Instagram para: {Username}", username);
            var runId = await _crowlerService.RunInstagramScraperAsync(username, limit);
            if (string.IsNullOrEmpty(runId))
            {
                _logger.LogError("Falha ao iniciar scraper do Instagram");
                return null;
            }
            _logger.LogInformation("Scraper do Instagram iniciado com sucesso. RunId: {RunId}", runId);
            return runId;
        }

        public async Task<IEnumerable<InstagramPost>> ProcessDatasetAsync(string datasetId)
        {
            _logger.LogInformation("Processando dataset: {DatasetId}", datasetId);
            var dataResult = await _crowlerService.ProcessInstagramDatasetAsync(datasetId);

            if (dataResult == null || !dataResult.Posts.Any())
            {
                _logger.LogWarning("Nenhum post encontrado no dataset: {DatasetId}", datasetId);
                // Retornamos uma lista vazia para indicar que não há nada a processar
                return Enumerable.Empty<InstagramPost>();
            }

            _logger.LogInformation("Classificando {PostsCount} posts", dataResult.Posts.Count());
            foreach (var post in dataResult.Posts)
            {
                post.Topic = await _postClassifierService.ClassifyPostAsync(post.Caption ?? "");
            }

            await _instagramRepository.SavePostsAsync(dataResult.Posts);

            if (dataResult.Comments.Any())
            {
                await _instagramRepository.SaveCommentsAsync(dataResult.Comments);
            }

            if (dataResult.Hashtags.Any())
            {
                await _instagramRepository.SaveHashtagsAsync(dataResult.Hashtags);
            }

            if (dataResult.Mentions.Any())
            {
                await _instagramRepository.SaveMentionsAsync(dataResult.Mentions);
            }

            _logger.LogInformation("Todos os dados do dataset {DatasetId} foram processados e salvos.", datasetId);
            return dataResult.Posts;
        }

        public async Task<IEnumerable<InstagramPost>> GetAllPostsAsync()
        {
            _logger.LogInformation("Buscando todos os posts do Instagram");
            return await _instagramRepository.GetAllPostsAsync();
        }

        public async Task<InstagramPost?> GetPostByIdAsync(string id)
        {
            _logger.LogInformation("Buscando post do Instagram com ID: {Id}", id);
            return await _instagramRepository.GetPostByIdAsync(id);
        }

        public async Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId)
        {
            _logger.LogInformation("Buscando comentários do post: {PostId}", postId);
            return await _instagramRepository.GetCommentsByPostIdAsync(postId);
        }

        public async Task<IEnumerable<InstagramHashtag>> GetHashtagsByPostIdAsync(string postId)
        {
            _logger.LogInformation("Buscando hashtags do post: {PostId}", postId);
            return await _instagramRepository.GetHashtagsByPostIdAsync(postId);
        }

        public async Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId)
        {
            _logger.LogInformation("Buscando menções do post: {PostId}", postId);
            return await _instagramRepository.GetMentionsByPostIdAsync(postId);
        }

        public async Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            return await _instagramRepository.SearchPostsByKeywordsAsync(keywords);
        }
    }
}