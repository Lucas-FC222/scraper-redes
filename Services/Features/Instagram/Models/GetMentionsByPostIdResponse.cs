namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de menções de um post do Instagram.
    /// </summary>
    public class GetMentionsByPostIdResponse
    {
        /// <summary>
        /// Coleção de menções do post do Instagram.
        /// </summary>
        public IEnumerable<InstagramMention> Mentions { get; set; } = [];
    }
}
