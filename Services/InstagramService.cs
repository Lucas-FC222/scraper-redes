using Infra;
using Infra.Data;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class InstagramService : IInstagramService
    {
        private readonly IApiFyService _apiFyService;
        private readonly IInstagramRepository _instagramRepository;
        private readonly ILogger<InstagramService> _logger;

        public InstagramService(IApiFyService apiFyService, IInstagramRepository instagramRepository, ILogger<InstagramService> logger)
        {
            _apiFyService = apiFyService;
            _instagramRepository = instagramRepository;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Iniciando serviço do Instagram");
                _logger.LogInformation("Serviço do Instagram executado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar serviço do Instagram");
            }
        }

        public async Task<string?> RunScraperAsync(string username, int limit)
        {
            try
            {
                _logger.LogInformation("Executando scraper do Instagram para: {Username}", username);
                var runId = await _apiFyService.RunInstagramScraperAsync(username, limit);
                if (string.IsNullOrEmpty(runId))
                {
                    _logger.LogError("Falha ao iniciar scraper do Instagram");
                    return null;
                }
                _logger.LogInformation("Scraper do Instagram iniciado com sucesso. RunId: {RunId}", runId);
                return runId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar scraper do Instagram");
                return null;
            }
        }

        public async Task<IEnumerable<Core.InstagramPost>> ProcessDatasetAsync(string datasetId)
        {
            try
            {
                _logger.LogInformation("Processando dataset: {DatasetId}", datasetId);
                var dataResult = await _apiFyService.ProcessInstagramDatasetAsync(datasetId);
                if (dataResult == null || !dataResult.Posts.Any())
                {
                    _logger.LogWarning("Nenhum post encontrado no dataset: {DatasetId}", datasetId);
                    return null;
                }
                
                _logger.LogInformation("Salvando {PostsCount} posts no banco de dados", dataResult.Posts.Count());
                await _instagramRepository.SavePostsAsync(dataResult.Posts);
                
                // Salvar comentários, hashtags e menções
                if (dataResult.Comments.Any())
                {
                    _logger.LogInformation("Salvando {CommentsCount} comentários no banco de dados", dataResult.Comments.Count());
                    await _instagramRepository.SaveCommentsAsync(dataResult.Comments);
                }
                
                if (dataResult.Hashtags.Any())
                {
                    _logger.LogInformation("Salvando {HashtagsCount} hashtags no banco de dados", dataResult.Hashtags.Count());
                    await _instagramRepository.SaveHashtagsAsync(dataResult.Hashtags);
                }
                
                if (dataResult.Mentions.Any())
                {
                    _logger.LogInformation("Salvando {MentionsCount} menções no banco de dados", dataResult.Mentions.Count());
                    await _instagramRepository.SaveMentionsAsync(dataResult.Mentions);
                }
                
                _logger.LogInformation("Todos os dados salvos com sucesso no banco de dados");
                return dataResult.Posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar dataset: {DatasetId}", datasetId);
                return null;
            }
        }

        public async Task<IEnumerable<Core.InstagramPost>> GetAllPostsAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todos os posts do Instagram");
                return await _instagramRepository.GetAllPostsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts do Instagram");
                return Enumerable.Empty<Core.InstagramPost>().ToList();
            }
        }

        public async Task<Core.InstagramPost?> GetPostByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Buscando post do Instagram com ID: {Id}", id);
                return await _instagramRepository.GetPostByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post do Instagram com ID: {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Core.InstagramComment>> GetCommentsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando comentários do post: {PostId}", postId);
                return await _instagramRepository.GetCommentsByPostIdAsync(postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar comentários do post: {PostId}", postId);
                return Enumerable.Empty<Core.InstagramComment>();
            }
        }

        public async Task<IEnumerable<Core.InstagramHashtag>> GetHashtagsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando hashtags do post: {PostId}", postId);
                return await _instagramRepository.GetHashtagsByPostIdAsync(postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hashtags do post: {PostId}", postId);
                return Enumerable.Empty<Core.InstagramHashtag>();
            }
        }

        public async Task<IEnumerable<Core.InstagramMention>> GetMentionsByPostIdAsync(string postId)
        {
            try
            {
                _logger.LogInformation("Buscando menções do post: {PostId}", postId);
                return await _instagramRepository.GetMentionsByPostIdAsync(postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar menções do post: {PostId}", postId);
                return Enumerable.Empty<Core.InstagramMention>();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}