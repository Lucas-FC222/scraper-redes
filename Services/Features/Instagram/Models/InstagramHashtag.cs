using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa uma hashtag associada a um post do Instagram.
    /// </summary>
    public class InstagramHashtag
    {
        /// <summary>
        /// Identificador do post ao qual a hashtag pertence.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string PostId { get; set; } = string.Empty; // FK para InstagramPosts
        
        /// <summary>
        /// Valor da hashtag.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Hashtag { get; set; } = string.Empty;
        
        /// <summary>
        /// Data de criação do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Chave composta para identificação única (PostId + Hashtag).
        /// </summary>
        public string CompositeKey => $"{PostId}_{Hashtag}";
    }
}