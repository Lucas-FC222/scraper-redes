using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Requisição para autenticação de usuário (login).
    /// </summary>
    public class LoginRequest : IRequest<Result<LoginResponse>>
    {
        /// <summary>
        /// E-mail do usuário para login.
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// Senha do usuário para login.
        /// </summary>
        public string Password { get; set; } = "";
    }
}
