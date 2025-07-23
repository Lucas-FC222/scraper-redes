using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class InstagramPost
    {
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(50)")]
        public string ShortCode { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(max)")]
        public string Caption { get; set; } = string.Empty;
        
        // Hashtags e Mentions serão armazenadas em tabelas separadas
        
        [Column(TypeName = "nvarchar(500)")]
        public string Url { get; set; } = string.Empty;
        
        public int CommentsCount { get; set; }
        
        // Comentários serão armazenados em tabela separada
        
        public int DimensionsHeight { get; set; }
        public int DimensionsWidth { get; set; }
        
        [Column(TypeName = "nvarchar(1000)")]
        public string DisplayUrl { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(max)")]
        public string Images { get; set; } = string.Empty; // JSON array como string
        
        [Column(TypeName = "nvarchar(1000)")]
        public string VideoUrl { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(500)")]
        public string Alt { get; set; } = string.Empty;
        
        public int LikesCount { get; set; }
        public int VideoViewCount { get; set; }
        public int VideoPlayCount { get; set; }
        
        [Column(TypeName = "nvarchar(100)")]
        public string Timestamp { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(max)")]
        public string ChildPosts { get; set; } = string.Empty; // JSON array como string
        
        [Column(TypeName = "nvarchar(200)")]
        public string OwnerFullName { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerUsername { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerId { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar(50)")]
        public string ProductType { get; set; } = string.Empty;
        
        public decimal VideoDuration { get; set; }
        public bool IsSponsored { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string TaggedUsers { get; set; } = string.Empty; // JSON array como string
        
        [Column(TypeName = "nvarchar(max)")]
        public string MusicInfo { get; set; } = string.Empty; // JSON object como string
        
        [Column(TypeName = "nvarchar(max)")]
        public string CoauthorProducers { get; set; } = string.Empty; // JSON array como string
        
        public bool IsCommentsDisabled { get; set; }
        
        [Column(TypeName = "nvarchar(1000)")]
        public string InputUrl { get; set; } = string.Empty; // URL original do perfil

        [Column(TypeName = "nvarchar(100)")]
        public string Topic { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 