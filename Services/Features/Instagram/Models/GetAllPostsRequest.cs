using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para obter todos os posts do Instagram cadastrados.
    /// </summary>
    public class GetAllPostsRequest : IRequest<Result<GetAllPostsResponse>>
    {
        // Nenhum campo ou propriedade adicional.
    }
}
