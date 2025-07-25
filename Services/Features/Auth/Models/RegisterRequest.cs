using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Requisição para registro de um novo usuário na aplicação.
    /// </summary>
    public class RegisterRequest : IRequest<Result<RegisterResponse>>
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Email do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha do usuário.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
