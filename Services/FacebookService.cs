using Microsoft.Extensions.Logging;
using Services.Features.Facebook.Models;
using Services.Features.Facebook.Repositories;
using Shared.Services;

namespace Services
{
    /// <summary>
    /// Serviço responsável por operações relacionadas ao Facebook, incluindo scraping, processamento e busca de posts.
    /// </summary>
    public class FacebookService : IFacebookService
    {
        private readonly ICrowlerService _crowlerService;
        private readonly IFacebookRepository _facebookRepository;
        private readonly ILogger<FacebookService> _logger;
        private readonly IPostClassifierService _postClassifierService;

        /// <summary>
        /// Inicializa o FacebookService.
        /// </summary>
        /// <param name="crowlerService">Serviço de scraping externo.</param>
        /// <param name="facebookRepository">Repositório de posts do Facebook.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="postClassifierService">Serviço de classificação de posts.</param>
        public FacebookService(ICrowlerService crowlerService, IFacebookRepository facebookRepository, ILogger<FacebookService> logger, IPostClassifierService postClassifierService)
        {
            _crowlerService = crowlerService;
            _facebookRepository = facebookRepository;
            _logger = logger;
            _postClassifierService = postClassifierService;
        }

        /// <summary>
        /// Executa o scraper do Facebook para uma página específica.
        /// </summary>
        /// <param name="pageUrl">URL da página do Facebook.</param>
        /// <param name="maxPosts">Número máximo de posts a coletar.</param>
        /// <returns>RunId do scraper ou null se falhar.</returns>
        public async Task<string?> RunScraperAsync(string pageUrl, int maxPosts)
        {
            
        }

        /// <summary>
        /// Processa um dataset do Facebook e salva os posts processados.
        /// </summary>
        /// <param name="datasetId">ID do dataset a ser processado.</param>
        /// <returns>Lista de posts processados.</returns>
        public async Task<IEnumerable<FacebookPost>> ProcessDatasetAsync(string datasetId)
        {
            
        }

        /// <summary>
        /// Retorna todos os posts do Facebook salvos.
        /// </summary>
        /// <returns>Lista de posts do Facebook.</returns>
        public async Task<IEnumerable<FacebookPost>> GetAllPostsAsync()
        {
            _logger.LogInformation("Buscando todos os posts do Facebook");
            return await _facebookRepository.GetAllPostsAsync();
        }

        /// <summary>
        /// Busca um post do Facebook pelo seu ID.
        /// </summary>
        /// <param name="id">ID do post.</param>
        /// <returns>Post encontrado ou null.</returns>
        public async Task<FacebookPost?> GetPostByIdAsync(string id)
        {
            _logger.LogInformation("Buscando post do Facebook com ID: {Id}", id);
            return await _facebookRepository.GetPostByIdAsync(id);
        }

        /// <summary>
        /// Busca posts do Facebook por palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave para busca.</param>
        /// <returns>Lista de posts que correspondem às palavras-chave.</returns>
        public async Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords)
        {
            return await _facebookRepository.SearchPostsByKeywordsAsync(keywords);
        }
    }
}