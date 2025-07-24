namespace Core.Models
{
    public class FacebookData
    {
        public IEnumerable<FacebookPost> Posts { get; set; } = Enumerable.Empty<FacebookPost>();
    }
} 