namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de comentários de um post do Instagram.
    /// </summary>
    public class GetCommentsByPostIdResponse
    {
        /// <summary>
        /// Coleção de comentários do post do Instagram.
        /// </summary>
        public IEnumerable<InstagramComment> Comments { get; set; } = [];
    }
}
