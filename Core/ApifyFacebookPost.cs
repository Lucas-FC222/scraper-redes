using System.Text.Json.Serialization;

namespace Core
{
    public class ApifyFacebookPost
    {
        [JsonPropertyName("post_id")]
        public string PostId { get; set; } = string.Empty;
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonPropertyName("comments_count")]
        public int CommentsCount { get; set; }
        
        [JsonPropertyName("reactions_count")]
        public int ReactionsCount { get; set; }
        
        [JsonPropertyName("author")]
        public ApifyFacebookAuthor? Author { get; set; }
        
        [JsonPropertyName("image")]
        public object? Image { get; set; }
        
        [JsonPropertyName("video")]
        public object? Video { get; set; }
        
        [JsonPropertyName("attached_post_url")]
        public object? AttachedPostUrl { get; set; }
    }
    
    public class ApifyFacebookAuthor
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("profile_picture_url")]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
} 