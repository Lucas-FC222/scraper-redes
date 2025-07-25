namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a pesquisa de posts do Instagram por palavras-chave.
    /// </summary>
    public class SearchPostsByKeywordsResponse
    {
        /// <summary>
        /// Coleção de posts do Instagram que correspondem às palavras-chave pesquisadas.
        /// </summary>
        public IEnumerable<InstagramPost> Posts { get; set; } = [];
    }
}
