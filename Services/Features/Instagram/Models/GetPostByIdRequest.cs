using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para obter um post do Instagram pelo identificador.
    /// </summary>
    public class GetPostByIdRequest : IRequest<Result<GetPostByIdResponse>>
    {
        /// <summary>
        /// Identificador do post do Instagram a ser consultado.
        /// </summary>
        public string Id { get; set; } = string.Empty;
    }
}
