using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class FacebookPost
    {
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(1000)")]
        public string Url { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(100)")]
        public string Topic { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; } = string.Empty;
        
        public long Timestamp { get; set; }
        
        public int CommentsCount { get; set; }
        
        public int ReactionsCount { get; set; }
        
        [Column(TypeName = "nvarchar(100)")]
        public string AuthorId { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(200)")]
        public string AuthorName { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthorUrl { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthorProfilePictureUrl { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(max)")]
        public string Image { get; set; } = string.Empty; // JSON object como string
        
        [Column(TypeName = "nvarchar(max)")]
        public string Video { get; set; } = string.Empty; // JSON object como string
        
        [Column(TypeName = "nvarchar(max)")]
        public string AttachedPostUrl { get; set; } = string.Empty; // JSON object como string
        
        [Column(TypeName = "nvarchar(1000)")]
        public string PageUrl { get; set; } = string.Empty; // URL da página original
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 