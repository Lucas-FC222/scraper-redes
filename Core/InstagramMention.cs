using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class InstagramMention
    {
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUsername { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUserId { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(200)")]
        public string MentionedFullName { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(500)")]
        public string MentionedProfilePicUrl { get; set; } = string.Empty;
        
        public bool IsVerified { get; set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Chave composta: PostId + MentionedUsername
        public string CompositeKey => $"{PostId}_{MentionedUsername}";
    }
} 