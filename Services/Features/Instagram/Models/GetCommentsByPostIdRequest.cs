using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para obter os comentários de um post do Instagram pelo identificador do post.
    /// </summary>
    public class GetCommentsByPostIdRequest : IRequest<Result<GetCommentsByPostIdResponse>>
    {
        /// <summary>
        /// Identificador do post do Instagram.
        /// </summary>
        public string PostId { get; set; } = string.Empty;
    }
}
