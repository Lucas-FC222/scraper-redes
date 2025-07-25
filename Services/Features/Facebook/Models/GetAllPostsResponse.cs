namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de todos os posts do Facebook.
    /// </summary>
    public class GetAllPostsResponse
    {
        /// <summary>
        /// Coleção de posts do Facebook retornados na resposta.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = [];
    }
}
