using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Features.Instagram.Models;
using Shared.Domain.Models;

namespace Api.Controllers
{
    /// <summary>
    /// Controller responsável por operações relacionadas ao Instagram, como scraping e consulta de posts.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ApiControllerBase
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="InstagramController"/>.
        /// </summary>
        /// <param name="mediator">Instância do MediatR injetada.</param>
        public InstagramController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Executa o scraper para um perfil do Instagram.
        /// </summary>
        /// <param name="request">Dados para execução do scraper.</param>
        /// <returns>Resultado da execução do scraper.</returns>
        [HttpPost("scraper")]
        [ProducesResponseType(typeof(Result<RunScraperResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<RunScraperResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunScraperAsync([FromBody] RunScraperRequest request)
        {
            var result = await _mediator.Send(request);
            return GetActionResult(result);
        }

        /// <summary>
        /// Obtém todos os posts do Instagram cadastrados.
        /// </summary>
        /// <returns>Lista de posts do Instagram.</returns>
        [HttpGet("posts")]
        [ProducesResponseType(typeof(Result<GetAllPostsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<GetAllPostsResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostsAsync()
        {
            var result = await _mediator.Send(new GetAllPostsRequest());
            return GetActionResult(result);
        }

        /// <summary>
        /// Obtém um post do Instagram pelo identificador.
        /// </summary>
        /// <param name="request">Dados para consulta do post por ID.</param>
        /// <returns>Post do Instagram correspondente ao ID informado.</returns>
        [HttpGet("posts/{id}")]
        [ProducesResponseType(typeof(Result<GetPostByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<GetPostByIdResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostByIdAsync([FromQuery] GetPostByIdRequest request)
        {
            var result = await _mediator.Send(request);
            return GetActionResult(result);
        }

        /// <summary>
        /// Pesquisa posts do Instagram por palavras-chave.
        /// </summary>
        /// <param name="request">Dados para pesquisa de posts por palavras-chave.</param>
        /// <returns>Lista de posts que correspondem às palavras-chave.</returns>
        [HttpGet("posts/search")]
        [ProducesResponseType(typeof(Result<SearchPostsByKeywordsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<SearchPostsByKeywordsResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<SearchPostsByKeywordsResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchPostsByKeywordsAsync([FromQuery] SearchPostsByKeywordsRequest request)
        {
            //if (string.IsNullOrWhiteSpace(keywords))
            //    return BadRequestResult<IEnumerable<InstagramPost>>("O parâmetro 'keywords' é obrigatório.");

            //var keywordsList = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            //if (!keywordsList.Any())
            //    return BadRequestResult<IEnumerable<InstagramPost>>("Nenhuma palavra-chave válida informada.");

            var result = await _mediator.Send(request);
            return GetActionResult(result);
        }
    }
}
