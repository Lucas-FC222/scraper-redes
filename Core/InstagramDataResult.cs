namespace Core
{
    public class InstagramDataResult
    {
        public IEnumerable<InstagramPost> Posts { get; set; } = Enumerable.Empty<InstagramPost>();
        public IEnumerable<InstagramComment> Comments { get; set; } = Enumerable.Empty<InstagramComment>();
        public IEnumerable<InstagramHashtag> Hashtags { get; set; } = Enumerable.Empty<InstagramHashtag>();
        public IEnumerable<InstagramMention> Mentions { get; set; } = Enumerable.Empty<InstagramMention>();
    }
} 