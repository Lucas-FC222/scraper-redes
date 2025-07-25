using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Features.Auth.Models;
using Shared.Domain.Models;

namespace Api.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações de autenticação, como login e registro de usuários.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="AuthController"/>.
        /// </summary>
        /// <param name="mediator">Instância do MediatR injetada.</param>
        public AuthController(IMediator mediator)
            : base(mediator)
        {
            
        }

        /// <summary>
        /// Realiza o login do usuário e retorna o token JWT em caso de sucesso.
        /// </summary>
        /// <param name="request">Dados de login do usuário.</param>
        /// <returns>Resultado da autenticação, incluindo token JWT e informações do usuário.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _mediator.Send(request);
            return GetActionResult(result);
        }

        /// <summary>
        /// Realiza o registro de um novo usuário e retorna o token JWT em caso de sucesso.
        /// </summary>
        /// <param name="request">Dados de registro do usuário.</param>
        /// <returns>Resultado do registro, incluindo token JWT e informações do usuário.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await _mediator.Send(request);
            return GetActionResult(result);
        }
    }
}
