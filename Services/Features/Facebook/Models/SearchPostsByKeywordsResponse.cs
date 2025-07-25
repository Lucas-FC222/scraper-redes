namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Resposta para a pesquisa de posts do Facebook por palavras-chave.
    /// </summary>
    public class SearchPostsByKeywordsResponse
    {
        /// <summary>
        /// Coleção de posts do Facebook que correspondem às palavras-chave pesquisadas.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = [];
    }
}
