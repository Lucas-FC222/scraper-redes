using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Requisição para obter todos os posts do Facebook cadastrados.
    /// </summary>
    public class GetAllPostsRequest : IRequest<Result<GetAllPostsResponse>>
    {
        // Nenhum campo ou propriedade adicional.
    }
}
