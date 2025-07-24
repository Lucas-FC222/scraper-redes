using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class InstagramComment
    {
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerUsername { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerId { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(500)")]
        public string OwnerProfilePicUrl { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string Timestamp { get; set; } = string.Empty;
        
        public int RepliesCount { get; set; }
        public int LikesCount { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string Replies { get; set; } = string.Empty; // JSON array como string
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 