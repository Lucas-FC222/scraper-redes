using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Models;

namespace Api.Controllers
{
    /// <summary>
    /// Controller base que inclui métodos para retornar resultados padronizados
    /// </summary>
    [ApiController]
    [Authorize]  // Requer autenticação para todos os controllers
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Cria um resultado de sucesso com dados
        /// </summary>
        /// <typeparam name="T">Tipo dos dados</typeparam>
        /// <param name="data">Dados para retornar</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
        protected IActionResult Success<T>(T data)
        {
            return Ok(Result<T>.Ok(data));
        }

        /// <summary>
        /// Cria um resultado de sucesso sem dados
        /// </summary>
        /// <returns>Um objeto ActionResult com o resultado</returns>
        protected IActionResult Success()
        {
            return Ok(Result.Ok());
        }

        /// <summary>
        /// Cria um resultado de erro NotFound (404)
        /// </summary>
        /// <typeparam name="T">Tipo esperado de dados</typeparam>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
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
        /// Cria um resultado de erro BadRequest (400)
        /// </summary>
        /// <typeparam name="T">Tipo esperado de dados</typeparam>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
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
        /// Cria um resultado de erro Unauthorized (401)
        /// </summary>
        /// <typeparam name="T">Tipo esperado de dados</typeparam>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
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
        /// Cria um resultado de erro Forbidden (403)
        /// </summary>
        /// <typeparam name="T">Tipo esperado de dados</typeparam>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
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
        /// Cria um resultado de erro InternalServerError (500)
        /// </summary>
        /// <typeparam name="T">Tipo esperado de dados</typeparam>
        /// <param name="message">Mensagem de erro</param>
        /// <returns>Um objeto ActionResult com o resultado</returns>
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
    }
}
