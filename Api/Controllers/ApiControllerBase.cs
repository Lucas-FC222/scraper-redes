using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Models;

namespace Api.Controllers
{
    /// <summary>
    /// Classe base abstrata para todos os controllers da API.
    /// Fornece métodos utilitários para padronizar respostas e tratamento de erros.
    /// </summary>
    [ApiController]
    [Authorize]  // Requer autenticação para todos os controllers
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Instância do MediatR para envio de comandos e queries.
        /// </summary>
        protected readonly IMediator _mediator;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ApiControllerBase"/>.
        /// </summary>
        /// <param name="mediator">Instância do MediatR injetada.</param>
        protected ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retorna uma resposta de sucesso com dados.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados retornados.</typeparam>
        /// <param name="data">Dados a serem retornados.</param>
        /// <returns>Resposta HTTP 200 com dados padronizados.</returns>
        protected IActionResult Success<T>(T data)
        {
            return Ok(Result<T>.Ok(data));
        }

        /// <summary>
        /// Retorna uma resposta de sucesso sem dados.
        /// </summary>
        /// <returns>Resposta HTTP 200 sem dados.</returns>
        protected IActionResult Success()
        {
            return Ok(Result.Ok());
        }

        /// <summary>
        /// Retorna uma resposta de recurso não encontrado (404) com mensagem personalizada.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado esperado.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta HTTP 404 com detalhes do problema.</returns>
        protected IActionResult NotFoundResult<T>(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso não encontrado",
                Detail = message,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/404"
            };

            return NotFound(Result<T>.Fail(problemDetails));
        }

        /// <summary>
        /// Retorna uma resposta de requisição inválida (400) com mensagem personalizada.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado esperado.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta HTTP 400 com detalhes do problema.</returns>
        protected IActionResult BadRequestResult<T>(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisição inválida",
                Detail = message,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/400"
            };

            return BadRequest(Result<T>.Fail(problemDetails));
        }

        /// <summary>
        /// Retorna uma resposta de não autorizado (401) com mensagem personalizada.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado esperado.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta HTTP 401 com detalhes do problema.</returns>
        protected IActionResult UnauthorizedResult<T>(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Não autorizado",
                Detail = message,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/401"
            };

            var result = Result<T>.Fail(problemDetails);
            return Unauthorized(result);
        }

        /// <summary>
        /// Retorna uma resposta de acesso proibido (403) com mensagem personalizada.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado esperado.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta HTTP 403 com detalhes do problema.</returns>
        protected IActionResult ForbiddenResult<T>(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Acesso proibido",
                Detail = message,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/403"
            };

            var result = Result<T>.Fail(problemDetails);
            return StatusCode(StatusCodes.Status403Forbidden, result);
        }

        /// <summary>
        /// Retorna uma resposta de erro interno do servidor (500) com mensagem personalizada.
        /// </summary>
        /// <typeparam name="T">Tipo do resultado esperado.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta HTTP 500 com detalhes do problema.</returns>
        protected IActionResult ServerErrorResult<T>(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno do servidor",
                Detail = message,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/500"
            };

            var result = Result<T>.Fail(problemDetails);
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        /// <summary>
        /// Retorna o <see cref="IActionResult"/> apropriado baseado no resultado padronizado.
        /// </summary>
        /// <typeparam name="T">Tipo do dado do resultado.</typeparam>
        /// <param name="result">Resultado padronizado.</param>
        /// <returns>Resposta HTTP correspondente ao status do resultado.</returns>
        protected IActionResult GetActionResult<T>(Result<T> result)
        {
            if (result.Success)
            {
                return Ok(result);
            }
            switch (result?.Error?.Status)
            {
                case StatusCodes.Status400BadRequest:
                    return BadRequestResult<string>(result?.Error?.Detail!);
                case StatusCodes.Status401Unauthorized:
                    return UnauthorizedResult<string>(result?.Error?.Detail!);
                case StatusCodes.Status403Forbidden:
                    return ForbiddenResult<string>(result?.Error?.Detail!);
                case StatusCodes.Status404NotFound:
                    return NotFoundResult<string>(result?.Error?.Detail!);
                case StatusCodes.Status500InternalServerError:
                    return ServerErrorResult<string>(result?.Error?.Detail!);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
