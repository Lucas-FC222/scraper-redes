using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para pesquisar posts do Instagram por palavras-chave.
    /// </summary>
    public class SearchPostsByKeywordsRequest : IRequest<Result<SearchPostsByKeywordsResponse>>
    {
        /// <summary>
        /// Lista de palavras-chave utilizadas na pesquisa de posts.
        /// </summary>
        public IEnumerable<string> Keywords { get; set; } = [];
    }
}
