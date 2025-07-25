using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um post do Instagram coletado pelo scraper, incluindo informações detalhadas e metadados.
    /// </summary>
    public class InstagramPost
    {
        /// <summary>
        /// Identificador único do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Tipo do post (ex: imagem, vídeo).
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Código curto do post.
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string ShortCode { get; set; } = string.Empty;
        /// <summary>
        /// Legenda do post.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Caption { get; set; } = string.Empty;
        // Hashtags e Mentions serão armazenadas em tabelas separadas
        /// <summary>
        /// URL do post no Instagram.
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de comentários.
        /// </summary>
        public int CommentsCount { get; set; }
        // Comentários serão armazenados em tabela separada
        /// <summary>
        /// Altura da mídia.
        /// </summary>
        public int DimensionsHeight { get; set; }
        /// <summary>
        /// Largura da mídia.
        /// </summary>
        public int DimensionsWidth { get; set; }
        /// <summary>
        /// URL da imagem de exibição.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string DisplayUrl { get; set; } = string.Empty;
        /// <summary>
        /// Imagens do post (JSON array como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string Images { get; set; } = string.Empty; // JSON array como string
        /// <summary>
        /// URL do vídeo do post.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string VideoUrl { get; set; } = string.Empty;
        /// <summary>
        /// Texto alternativo da imagem/vídeo.
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string Alt { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de curtidas.
        /// </summary>
        public int LikesCount { get; set; }
        /// <summary>
        /// Quantidade de visualizações do vídeo.
        /// </summary>
        public int VideoViewCount { get; set; }
        /// <summary>
        /// Quantidade de reproduções do vídeo.
        /// </summary>
        public int VideoPlayCount { get; set; }
        /// <summary>
        /// Timestamp do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Timestamp { get; set; } = string.Empty;
        /// <summary>
        /// Posts filhos (JSON array como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string ChildPosts { get; set; } = string.Empty; // JSON array como string
        /// <summary>
        /// Nome completo do proprietário do post.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string OwnerFullName { get; set; } = string.Empty;
        /// <summary>
        /// Username do proprietário do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerUsername { get; set; } = string.Empty;
        /// <summary>
        /// Id do proprietário do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string OwnerId { get; set; } = string.Empty;
        /// <summary>
        /// Tipo de produto do post.
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string ProductType { get; set; } = string.Empty;
        /// <summary>
        /// Duração do vídeo.
        /// </summary>
        public decimal VideoDuration { get; set; }
        /// <summary>
        /// Indica se o post é patrocinado.
        /// </summary>
        public bool IsSponsored { get; set; }
        /// <summary>
        /// Usuários marcados (JSON array como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string TaggedUsers { get; set; } = string.Empty; // JSON array como string
        /// <summary>
        /// Informações de música (JSON object como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string MusicInfo { get; set; } = string.Empty; // JSON object como string
        /// <summary>
        /// Coautores/produtores (JSON array como string).
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string CoauthorProducers { get; set; } = string.Empty; // JSON array como string
        /// <summary>
        /// Indica se os comentários estão desabilitados.
        /// </summary>
        public bool IsCommentsDisabled { get; set; }
        /// <summary>
        /// URL original do perfil.
        /// </summary>
        [Column(TypeName = "nvarchar(1000)")]
        public string InputUrl { get; set; } = string.Empty; // URL original do perfil
        /// <summary>
        /// Tópico classificado do post.
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string Topic { get; set; } = string.Empty;
        /// <summary>
        /// Data de criação do registro no sistema.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}