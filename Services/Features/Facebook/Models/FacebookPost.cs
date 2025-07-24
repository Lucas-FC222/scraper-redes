using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa um post do Facebook coletado pelo scraper.
    /// </summary>
    public class FacebookPost
    {
        /// <summary>
        /// Identificador único do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// URL do post no Facebook.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Tópico classificado do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Topic { get; set; } = string.Empty;
        /// <summary>
        /// Mensagem/texto do post.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp do post (Unix time).
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// Quantidade de comentários.
        /// </summary>
        public int CommentsCount { get; set; }
        /// <summary>
        /// Quantidade de reações.
        /// </summary>
        public int ReactionsCount { get; set; }
        /// <summary>
        /// Id do autor do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string AuthorId { get; set; } = string.Empty;
        /// <summary>
        /// Nome do autor do post.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string AuthorName { get; set; } = string.Empty;
        /// <summary>
        /// URL do perfil do autor.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthorUrl { get; set; } = string.Empty;
        /// <summary>
        /// URL da foto de perfil do autor.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthorProfilePictureUrl { get; set; } = string.Empty;
        /// <summary>
        /// Imagem do post (JSON como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Image { get; set; } = string.Empty; // JSON object como string
        /// <summary>
        /// Vídeo do post (JSON como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Video { get; set; } = string.Empty; // JSON object como string
        /// <summary>
        /// URL de post anexado (JSON como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string AttachedPostUrl { get; set; } = string.Empty; // JSON object como string
        /// <summary>
        /// URL da página do Facebook de onde o post foi coletado.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string PageUrl { get; set; } = string.Empty; // URL da página original
        /// <summary>
        /// Data de criação do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 