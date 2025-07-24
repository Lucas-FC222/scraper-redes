using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Features.Auth.Models;
using Shared.Services;

namespace Api.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e autorização de usuários
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous] // Permite acesso anônimo para login e registro
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Construtor do AuthController
        /// </summary>
        /// <param name="authService">Serviço de autenticação</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica um usuário com email e senha
        /// </summary>
        /// <param name="loginDto">DTO com email e senha do usuário</param>
        /// <returns>Token JWT para autenticação e informações adicionais</returns>
        /// <response code="200">Retorna o token JWT e informações do usuário quando o login é bem-sucedido</response>
        /// <response code="400">Retorna mensagem de erro quando as credenciais são inválidas</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(typeof(AuthResultDto), 400)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registerDto">DTO com dados para registro do usuário (nome, email e senha)</param>
        /// <returns>Token JWT e confirmação do registro bem-sucedido</returns>
        /// <response code="200">Retorna o token JWT quando o registro é bem-sucedido</response>
        /// <response code="400">Retorna mensagem de erro quando o registro falha (ex: email já em uso)</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(typeof(AuthResultDto), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Endpoint protegido que requer autenticação
        /// </summary>
        /// <returns>Mensagem de confirmação de acesso à rota protegida</returns>
        /// <response code="200">Retorna mensagem confirmando acesso ao conteúdo protegido</response>
        /// <response code="401">Retorna erro quando o usuário não está autenticado</response>
        [HttpGet("protected")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public IActionResult Protected()
        {
            return Ok(new { message = "Esta é uma rota protegida" });
        }

        /// <summary>
        /// Endpoint restrito apenas para usuários com papel de Administrador
        /// </summary>
        /// <returns>Mensagem de confirmação de acesso à rota administrativa</returns>
        /// <response code="200">Retorna mensagem confirmando acesso ao conteúdo administrativo</response>
        /// <response code="401">Retorna erro quando o usuário não está autenticado</response>
        /// <response code="403">Retorna erro quando o usuário não tem permissão de administrador</response>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Esta é uma rota apenas para administradores" });
        }
    }
}
