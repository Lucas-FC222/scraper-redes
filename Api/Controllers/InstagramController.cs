using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly ILogger<InstagramController> _logger;

        public InstagramController(IInstagramService instagramService, ILogger<InstagramController> logger)
        {
            _instagramService = instagramService;
            _logger = logger;
        }

        [HttpPost("scraper")]
        public async Task<IActionResult> RunScraper([FromBody] InstagramScraperRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando scraper do Instagram para username: {Username}, limite: {Limit}", request.Username, request.Limit);
                var runId = await _instagramService.RunScraperAsync(request.Username, request.Limit);
                if (string.IsNullOrEmpty(runId))
                {
                    return BadRequest("Falha ao iniciar scraper do Instagram");
                }
                return Ok(new { RunId = runId, Message = "Scraper do Instagram iniciado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar scraper do Instagram");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        public class InstagramScraperRequest
        {
            public string Username { get; set; } = string.Empty;
            public int Limit { get; set; } = 10;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                _logger.LogInformation("Buscando todos os posts do Instagram");
                
                var posts = await _instagramService.GetAllPostsAsync();
                
                _logger.LogInformation("Encontrados {Count} posts do Instagram", posts.Count());
                
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts do Instagram");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            try
            {
                _logger.LogInformation("Buscando post do Instagram com ID: {Id}", id);
                
                var post = await _instagramService.GetPostByIdAsync(id);
                
                if (post == null)
                {
                    _logger.LogWarning("Post não encontrado para ID: {Id}", id);
                    return NotFound($"Post com ID {id} não encontrado");
                }

                _logger.LogInformation("Post encontrado para ID: {Id}", id);
                
                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post do Instagram com ID: {Id}", id);
                return StatusCode(500, $"Erro interno: {ex.Message}");
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

            var posts = await _instagramService.SearchPostsByKeywordsAsync(keywordsList);
            return Ok(posts);
        }
    }
}
