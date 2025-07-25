using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa uma menção a um usuário em um post do Instagram.
    /// </summary>
    public class InstagramMention
    {
        /// <summary>
        /// Identificador do post ao qual a menção pertence.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        /// <summary>
        /// Nome de usuário mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUsername { get; set; } = string.Empty;
        
        /// <summary>
        /// Identificador do usuário mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Nome completo do usuário mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string MentionedFullName { get; set; } = string.Empty;
        
        /// <summary>
        /// URL da foto de perfil do usuário mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string MentionedProfilePicUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica se o usuário mencionado é verificado.
        /// </summary>
        public bool IsVerified { get; set; }
        
        /// <summary>
        /// Data de criação do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Chave composta para identificação única (PostId + MentionedUsername).
        /// </summary>
        public string CompositeKey => $"{PostId}_{MentionedUsername}";
    }
}