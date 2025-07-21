namespace Core
{
    public class FacebookDataResult
    {
        public IEnumerable<FacebookPost> Posts { get; set; } = Enumerable.Empty<FacebookPost>();
    }
} 