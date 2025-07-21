using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class InstagramHashtag
    {
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        [Column(TypeName = "nvarchar(100)")]
        public string Hashtag { get; set; } = string.Empty;
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Chave composta: PostId + Hashtag
        public string CompositeKey => $"{PostId}_{Hashtag}";
    }
} 