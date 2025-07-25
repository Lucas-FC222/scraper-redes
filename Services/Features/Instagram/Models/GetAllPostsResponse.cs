namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de todos os posts do Instagram.
    /// </summary>
    public class GetAllPostsResponse
    {
        /// <summary>
        /// Coleção de posts do Instagram retornados na resposta.
        /// </summary>
        public IEnumerable<InstagramPost> Posts { get; set; } = [];
    }
}
