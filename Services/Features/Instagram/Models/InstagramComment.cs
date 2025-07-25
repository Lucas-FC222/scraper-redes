using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um comentário de um post do Instagram armazenado no sistema.
    /// </summary>
    public class InstagramComment
    {
        /// <summary>
        /// Identificador único do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Identificador do post ao qual o comentário pertence.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        /// <summary>
        /// Texto do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usuário do autor do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerUsername { get; set; } = string.Empty;
        /// <summary>
        /// Identificador do autor do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerId { get; set; } = string.Empty;
        /// <summary>
        /// URL da foto de perfil do autor do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string OwnerProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp do comentário.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Timestamp { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de respostas ao comentário.
        /// </summary>
        public int RepliesCount { get; set; }
        /// <summary>
        /// Quantidade de curtidas no comentário.
        /// </summary>
        public int LikesCount { get; set; }
        /// <summary>
        /// Respostas ao comentário (JSON array como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Replies { get; set; } = string.Empty; // JSON array como string
        /// <summary>
        /// Data de criação do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}