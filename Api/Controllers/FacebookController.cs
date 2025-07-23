using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacebookController : ControllerBase
    {
        private readonly IFacebookService _facebookService;
        private readonly ILogger<FacebookController> _logger;

        public FacebookController(IFacebookService facebookService, ILogger<FacebookController> logger)
        {
            _facebookService = facebookService;
            _logger = logger;
        }

        [HttpPost("scraper")]
        public async Task<IActionResult> RunScraper([FromBody] FacebookScraperRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando scraper do Facebook para página: {PageUrl}", request.PageUrl);
                
                var runId = await _facebookService.RunScraperAsync(request.PageUrl, request.MaxPosts);
                if (string.IsNullOrEmpty(runId))
                {
                    return BadRequest("Falha ao iniciar scraper do Facebook");
                }

                return Ok(new { RunId = runId, Message = "Scraper do Facebook iniciado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar scraper do Facebook");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _facebookService.GetAllPostsAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts do Facebook");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            try
            {
                var post = await _facebookService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }
                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post do Facebook com ID: {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("posts/search")]
        public async Task<IActionResult> SearchPostsByKeywords([FromQuery] string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return BadRequest("O parâmetro 'keywords' é obrigatório.");

            var keywordsList = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!keywordsList.Any())
                return BadRequest("Nenhuma palavra-chave válida informada.");

            var posts = await _facebookService.SearchPostsByKeywordsAsync(keywordsList);
            return Ok(posts);
        }
    }

    public class FacebookScraperRequest
    {
        public string PageUrl { get; set; } = string.Empty;
        public int MaxPosts { get; set; }
    }
}
