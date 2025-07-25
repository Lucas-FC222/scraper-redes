using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para obter as menções de um post do Instagram pelo identificador do post.
    /// </summary>
    public class GetMentionsByPostIdRequest : IRequest<Result<GetMentionsByPostIdResponse>>
    {
        /// <summary>
        /// Identificador do post do Instagram.
        /// </summary>
        public string PostId { get; set; } = string.Empty;
    }
}
