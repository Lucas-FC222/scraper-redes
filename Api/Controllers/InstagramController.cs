using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas ao Instagram
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ApiControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly ILogger<InstagramController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do controlador Instagram
        /// </summary>
        /// <param name="instagramService">Serviço para operações do Instagram</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public InstagramController(IInstagramService instagramService, ILogger<InstagramController> logger)
        {
            _instagramService = instagramService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia o scraper para coletar posts de um usuário do Instagram.
        /// </summary>
        /// <param name="request">Objeto contendo o username e o limite de posts.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Scraper iniciado com sucesso, retorna RunId e mensagem.</para>
        /// <para><b>400 BadRequest</b>: Falha ao iniciar scraper.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPost("scraper")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunScraper([FromBody] InstagramScraperRequest request)
        {
            _logger.LogInformation("Iniciando scraper do Instagram para username: {Username}, limite: {Limit}", request.Username, request.Limit);
            var runId = await _instagramService.RunScraperAsync(request.Username, request.Limit);
            if (string.IsNullOrEmpty(runId))
            {
                return BadRequestResult<object>("Falha ao iniciar scraper do Instagram");
            }
            return Success(new { RunId = runId, Message = "Scraper do Instagram iniciado com sucesso" });
        }

        /// <summary>
        /// Modelo para requisição de scraper do Instagram
        /// </summary>
        public class InstagramScraperRequest
        {
            /// <summary>
            /// Nome de usuário do Instagram para fazer scraping
            /// </summary>
            public string Username { get; set; } = string.Empty;
            
            /// <summary>
            /// Limite de posts a serem coletados
            /// </summary>
            public int Limit { get; set; } = 10;
        }

        /// <summary>
        /// Retorna todos os posts coletados do Instagram.
        /// </summary>
        /// <returns>
        /// <para><b>200 OK</b>: Lista de posts coletados.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpGet("posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts()
        {
            _logger.LogInformation("Buscando todos os posts do Instagram");
            
            var posts = await _instagramService.GetAllPostsAsync();
            
            _logger.LogInformation("Encontrados {Count} posts do Instagram", posts.Count());
            
            return Success(posts);
        }

        /// <summary>
        /// Retorna um post específico do Instagram pelo seu ID.
        /// </summary>
        /// <param name="id">ID do post.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Post encontrado.</para>
        /// <para><b>404 NotFound</b>: Post não encontrado.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpGet("posts/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostById(string id)
        {
            _logger.LogInformation("Buscando post do Instagram com ID: {Id}", id);
            
            var post = await _instagramService.GetPostByIdAsync(id);
            
            if (post == null)
            {
                _logger.LogWarning("Post não encontrado para ID: {Id}", id);
                return NotFoundResult<InstagramPost>($"Post com ID {id} não encontrado");
            }

            _logger.LogInformation("Post encontrado para ID: {Id}", id);
            
            return Success(post);
        }

        /// <summary>
        /// Busca posts do Instagram por palavras-chave.
        /// </summary>
        /// <param name="keywords">Palavras-chave separadas por vírgula.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Lista de posts que correspondem às palavras-chave.</para>
        /// <para><b>400 BadRequest</b>: Parâmetro 'keywords' ausente ou inválido.</para>
        /// </returns>
        [HttpGet("posts/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPostsByKeywords([FromQuery] string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return BadRequestResult<IEnumerable<InstagramPost>>("O parâmetro 'keywords' é obrigatório.");

            var keywordsList = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!keywordsList.Any())
                return BadRequestResult<IEnumerable<InstagramPost>>("Nenhuma palavra-chave válida informada.");

            var posts = await _instagramService.SearchPostsByKeywordsAsync(keywordsList);
            return Success(posts);
        }
    }
}
