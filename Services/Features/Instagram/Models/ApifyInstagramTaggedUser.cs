using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    public class ApifyInstagramTaggedUser
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        
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