using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Requisição para pesquisar posts do Facebook por palavras-chave.
    /// </summary>
    public class SearchPostsByKeywordsRequest : IRequest<Result<SearchPostsByKeywordsResponse>>
    {
        /// <summary>
        /// Lista de palavras-chave utilizadas na pesquisa de posts.
        /// </summary>
        public IEnumerable<string> Keywords { get; set; } = [];
    }
}
