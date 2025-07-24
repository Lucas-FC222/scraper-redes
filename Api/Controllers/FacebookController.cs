using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Services;

namespace Api.Controllers
{
    /// <summary>
    /// Controller para operações relacionadas ao Facebook
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FacebookController : ApiControllerBase
    {
        private readonly IFacebookService _facebookService;
        private readonly ILogger<FacebookController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do controlador Facebook
        /// </summary>
        /// <param name="facebookService">Serviço para operações do Facebook</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public FacebookController(IFacebookService facebookService, ILogger<FacebookController> logger)
        {
            _facebookService = facebookService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia o scraper para coletar posts de uma página do Facebook.
        /// </summary>
        /// <param name="request">Objeto contendo a URL da página e o número máximo de posts.</param>
        /// <returns>
        /// <para><b>200 OK</b>: Scraper iniciado com sucesso, retorna RunId e mensagem.</para>
        /// <para><b>400 BadRequest</b>: Falha ao iniciar o scraper.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpPost("scraper")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunScraper([FromBody] FacebookScraperRequest request)
        {
            _logger.LogInformation("Iniciando scraper do Facebook para página: {PageUrl}", request.PageUrl);
            
            var runId = await _facebookService.RunScraperAsync(request.PageUrl, request.MaxPosts);
            if (string.IsNullOrEmpty(runId))
            {
                return BadRequestResult<object>("Falha ao iniciar scraper do Facebook");
            }

            return Success(new { RunId = runId, Message = "Scraper do Facebook iniciado com sucesso" });
        }

        /// <summary>
        /// Retorna todos os posts coletados do Facebook.
        /// </summary>
        /// <returns>
        /// <para><b>200 OK</b>: Lista de posts coletados.</para>
        /// <para><b>500 InternalServerError</b>: Erro interno do servidor.</para>
        /// </returns>
        [HttpGet("posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _facebookService.GetAllPostsAsync();
            return Success(posts);
        }

        /// <summary>
        /// Retorna um post específico do Facebook pelo seu ID.
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
            var post = await _facebookService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFoundResult<FacebookPost>($"Post com ID '{id}' não encontrado");
            }
            return Success(post);
        }

        /// <summary>
        /// Busca posts do Facebook por palavras-chave.
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
                return BadRequestResult<IEnumerable<FacebookPost>>("O parâmetro 'keywords' é obrigatório.");

            var keywordsList = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!keywordsList.Any())
                return BadRequestResult<IEnumerable<FacebookPost>>("Nenhuma palavra-chave válida informada.");

            var posts = await _facebookService.SearchPostsByKeywordsAsync(keywordsList);
            return Success(posts);
        }
    }

    /// <summary>
    /// Modelo para requisição de scraper do Facebook
    /// </summary>
    public class FacebookScraperRequest
    {
        /// <summary>
        /// URL da página do Facebook para fazer scraping
        /// </summary>
        public string PageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Número máximo de posts a serem coletados
        /// </summary>
        public int MaxPosts { get; set; }
    }
}
