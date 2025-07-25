namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um conjunto de dados do Instagram contendo coleções de posts, comentários, hashtags e menções.
    /// </summary>
    public class InstagramData
    {
        /// <summary>
        /// Coleção de posts do Instagram.
        /// </summary>
        public IEnumerable<InstagramPost> Posts { get; set; } = Enumerable.Empty<InstagramPost>();
        /// <summary>
        /// Coleção de comentários do Instagram.
        /// </summary>
        public IEnumerable<InstagramComment> Comments { get; set; } = Enumerable.Empty<InstagramComment>();
        /// <summary>
        /// Coleção de hashtags do Instagram.
        /// </summary>
        public IEnumerable<InstagramHashtag> Hashtags { get; set; } = Enumerable.Empty<InstagramHashtag>();
        /// <summary>
        /// Coleção de menções do Instagram.
        /// </summary>
        public IEnumerable<InstagramMention> Mentions { get; set; } = Enumerable.Empty<InstagramMention>();
    }
}