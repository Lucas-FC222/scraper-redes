using System.Text.Json.Serialization;

namespace Core
{
    public class ApifyInstagramPost
    {
        [JsonPropertyName("inputUrl")]
        public string InputUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        
        [JsonPropertyName("shortCode")]
        public string ShortCode { get; set; } = string.Empty;
        
        [JsonPropertyName("caption")]
        public string Caption { get; set; } = string.Empty;
        
        [JsonPropertyName("hashtags")]
        public List<string> Hashtags { get; set; } = new();
        
        [JsonPropertyName("mentions")]
        public List<string> Mentions { get; set; } = new();
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("commentsCount")]
        public int CommentsCount { get; set; }
        
        [JsonPropertyName("firstComment")]
        public string FirstComment { get; set; } = string.Empty;
        
        [JsonPropertyName("latestComments")]
        public List<ApifyInstagramComment> LatestComments { get; set; } = new();
        
        [JsonPropertyName("dimensionsHeight")]
        public int DimensionsHeight { get; set; }
        
        [JsonPropertyName("dimensionsWidth")]
        public int DimensionsWidth { get; set; }
        
        [JsonPropertyName("displayUrl")]
        public string DisplayUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("images")]
        public List<object> Images { get; set; } = new();
        
        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("alt")]
        public string? Alt { get; set; }
        
        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; }
        
        [JsonPropertyName("videoViewCount")]
        public int VideoViewCount { get; set; }
        
        [JsonPropertyName("videoPlayCount")]
        public int VideoPlayCount { get; set; }
        
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        
        [JsonPropertyName("childPosts")]
        public List<object> ChildPosts { get; set; } = new();
        
        [JsonPropertyName("ownerFullName")]
        public string OwnerFullName { get; set; } = string.Empty;
        
        [JsonPropertyName("ownerUsername")]
        public string OwnerUsername { get; set; } = string.Empty;
        
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; } = string.Empty;
        
        [JsonPropertyName("productType")]
        public string ProductType { get; set; } = string.Empty;
        
        [JsonPropertyName("videoDuration")]
        public decimal VideoDuration { get; set; }
        
        [JsonPropertyName("isSponsored")]
        public bool IsSponsored { get; set; }
        
        [JsonPropertyName("taggedUsers")]
        public List<ApifyInstagramTaggedUser> TaggedUsers { get; set; } = new();
        
        [JsonPropertyName("musicInfo")]
        public object? MusicInfo { get; set; }
        
        [JsonPropertyName("coauthorProducers")]
        public List<object> CoauthorProducers { get; set; } = new();
        
        [JsonPropertyName("isCommentsDisabled")]
        public bool IsCommentsDisabled { get; set; }
    }
} 