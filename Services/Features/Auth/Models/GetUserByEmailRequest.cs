using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Requisição para obter um usuário pelo e-mail.
    /// </summary>
    public class GetUserByEmailRequest : IRequest<Result<GetUserByEmailResponse>>
    {
        /// <summary>
        /// E-mail do usuário a ser consultado.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
