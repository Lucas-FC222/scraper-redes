namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a requisição de obtenção de hashtags de um post do Instagram.
    /// </summary>
    public class GetHashTagsByPostIdResponse
    {
        /// <summary>
        /// Coleção de hashtags do post do Instagram.
        /// </summary>
        public IEnumerable<InstagramHashtag> HashTags { get; set; } = [];
    }
}
