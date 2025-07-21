using System.Text.Json.Serialization;

namespace Core
{
    public class ApifyInstagramComment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        
        [JsonPropertyName("ownerUsername")]
        public string OwnerUsername { get; set; } = string.Empty;
        
        [JsonPropertyName("ownerProfilePicUrl")]
        public string OwnerProfilePicUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        
        [JsonPropertyName("repliesCount")]
        public int RepliesCount { get; set; }
        
        [JsonPropertyName("replies")]
        public List<object> Replies { get; set; } = new();
        
        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; }
        
        [JsonPropertyName("owner")]
        public ApifyInstagramCommentOwner? Owner { get; set; }
    }
    
    public class ApifyInstagramCommentOwner
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }
        
        [JsonPropertyName("profile_pic_url")]
        public string ProfilePicUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
} 