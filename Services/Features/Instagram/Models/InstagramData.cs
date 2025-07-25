namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um conjunto de dados do Instagram contendo cole��es de posts, coment�rios, hashtags e men��es.
    /// </summary>
    public class InstagramData
    {
        /// <summary>
        /// Cole��o de posts do Instagram.
        /// </summary>
        public IEnumerable<InstagramPost> Posts { get; set; } = Enumerable.Empty<InstagramPost>();
        /// <summary>
        /// Cole��o de coment�rios do Instagram.
        /// </summary>
        public IEnumerable<InstagramComment> Comments { get; set; } = Enumerable.Empty<InstagramComment>();
        /// <summary>
        /// Cole��o de hashtags do Instagram.
        /// </summary>
        public IEnumerable<InstagramHashtag> Hashtags { get; set; } = Enumerable.Empty<InstagramHashtag>();
        /// <summary>
        /// Cole��o de men��es do Instagram.
        /// </summary>
        public IEnumerable<InstagramMention> Mentions { get; set; } = Enumerable.Empty<InstagramMention>();
    }
}