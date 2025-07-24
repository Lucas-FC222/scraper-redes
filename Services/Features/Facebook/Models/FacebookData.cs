namespace Services.Features.Facebook.Models
{
    public class FacebookData
    {
        public IEnumerable<FacebookPost> Posts { get; set; } = Enumerable.Empty<FacebookPost>();
    }
} 