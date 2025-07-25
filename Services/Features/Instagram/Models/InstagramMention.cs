using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa uma men��o a um usu�rio em um post do Instagram.
    /// </summary>
    public class InstagramMention
    {
        /// <summary>
        /// Identificador do post ao qual a men��o pertence.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        /// <summary>
        /// Nome de usu�rio mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUsername { get; set; } = string.Empty;
        
        /// <summary>
        /// Identificador do usu�rio mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string MentionedUserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Nome completo do usu�rio mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string MentionedFullName { get; set; } = string.Empty;
        
        /// <summary>
        /// URL da foto de perfil do usu�rio mencionado.
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string MentionedProfilePicUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica se o usu�rio mencionado � verificado.
        /// </summary>
        public bool IsVerified { get; set; }
        
        /// <summary>
        /// Data de cria��o do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Chave composta para identifica��o �nica (PostId + MentionedUsername).
        /// </summary>
        public string CompositeKey => $"{PostId}_{MentionedUsername}";
    }
}