using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Requisição para obter um post do Facebook pelo identificador.
    /// </summary>
    public class GetPostByIdRequest : IRequest<Result<GetPostByIdResponse>>
    {
        /// <summary>
        /// Identificador do post do Facebook a ser consultado.
        /// </summary>
        public string Id { get; set; } = string.Empty;
    }
}
