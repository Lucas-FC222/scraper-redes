using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um post do Instagram retornado pela API Apify, incluindo informações detalhadas do post e seus relacionamentos.
    /// </summary>
    public class ApifyInstagramPost
    {
        /// <summary>
        /// URL de entrada utilizada para coletar o post.
        /// </summary>
        [JsonPropertyName("inputUrl")]
        public string InputUrl { get; set; } = string.Empty;
        /// <summary>
        /// Identificador único do post.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Tipo do post (ex: imagem, vídeo).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Código curto do post.
        /// </summary>
        [JsonPropertyName("shortCode")]
        public string ShortCode { get; set; } = string.Empty;
        /// <summary>
        /// Legenda do post.
        /// </summary>
        [JsonPropertyName("caption")]
        public string Caption { get; set; } = string.Empty;
        /// <summary>
        /// Lista de hashtags presentes no post.
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<string> Hashtags { get; set; } = new();
        /// <summary>
        /// Lista de menções presentes no post.
        /// </summary>
        [JsonPropertyName("mentions")]
        public List<string> Mentions { get; set; } = new();
        /// <summary>
        /// URL do post no Instagram.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de comentários no post.
        /// </summary>
        [JsonPropertyName("commentsCount")]
        public int CommentsCount { get; set; }
        /// <summary>
        /// Primeiro comentário do post.
        /// </summary>
        [JsonPropertyName("firstComment")]
        public string FirstComment { get; set; } = string.Empty;
        /// <summary>
        /// Lista dos comentários mais recentes do post.
        /// </summary>
        [JsonPropertyName("latestComments")]
        public List<ApifyInstagramComment> LatestComments { get; set; } = new();
        /// <summary>
        /// Altura da mídia do post.
        /// </summary>
        [JsonPropertyName("dimensionsHeight")]
        public int DimensionsHeight { get; set; }
        /// <summary>
        /// Largura da mídia do post.
        /// </summary>
        [JsonPropertyName("dimensionsWidth")]
        public int DimensionsWidth { get; set; }
        /// <summary>
        /// URL da imagem de exibição do post.
        /// </summary>
        [JsonPropertyName("displayUrl")]
        public string DisplayUrl { get; set; } = string.Empty;
        /// <summary>
        /// Lista de imagens do post (pode conter objetos variados).
        /// </summary>
        [JsonPropertyName("images")]
        public List<object> Images { get; set; } = new();
        /// <summary>
        /// URL do vídeo do post.
        /// </summary>
        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;
        /// <summary>
        /// Texto alternativo da imagem/vídeo.
        /// </summary>
        [JsonPropertyName("alt")]
        public string? Alt { get; set; }
        /// <summary>
        /// Quantidade de curtidas no post.
        /// </summary>
        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; }
        /// <summary>
        /// Quantidade de visualizações do vídeo.
        /// </summary>
        [JsonPropertyName("videoViewCount")]
        public int VideoViewCount { get; set; }
        /// <summary>
        /// Quantidade de reproduções do vídeo.
        /// </summary>
        [JsonPropertyName("videoPlayCount")]
        public int VideoPlayCount { get; set; }
        /// <summary>
        /// Timestamp do post.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        /// <summary>
        /// Lista de posts filhos (ex: carrossel).
        /// </summary>
        [JsonPropertyName("childPosts")]
        public List<object> ChildPosts { get; set; } = new();
        /// <summary>
        /// Nome completo do proprietário do post.
        /// </summary>
        [JsonPropertyName("ownerFullName")]
        public string OwnerFullName { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usuário do proprietário do post.
        /// </summary>
        [JsonPropertyName("ownerUsername")]
        public string OwnerUsername { get; set; } = string.Empty;
        /// <summary>
        /// Id do proprietário do post.
        /// </summary>
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; } = string.Empty;
        /// <summary>
        /// Tipo de produto do post.
        /// </summary>
        [JsonPropertyName("productType")]
        public string ProductType { get; set; } = string.Empty;
        /// <summary>
        /// Duração do vídeo do post.
        /// </summary>
        [JsonPropertyName("videoDuration")]
        public decimal VideoDuration { get; set; }
        /// <summary>
        /// Indica se o post é patrocinado.
        /// </summary>
        [JsonPropertyName("isSponsored")]
        public bool IsSponsored { get; set; }
        /// <summary>
        /// Lista de usuários marcados no post.
        /// </summary>
        [JsonPropertyName("taggedUsers")]
        public List<ApifyInstagramTaggedUser> TaggedUsers { get; set; } = new();
        /// <summary>
        /// Informações de música associadas ao post (pode ser um objeto JSON).
        /// </summary>
        [JsonPropertyName("musicInfo")]
        public object? MusicInfo { get; set; }
        /// <summary>
        /// Lista de coautores/produtores do post.
        /// </summary>
        [JsonPropertyName("coauthorProducers")]
        public List<object> CoauthorProducers { get; set; } = new();
        /// <summary>
        /// Indica se os comentários estão desabilitados no post.
        /// </summary>
        [JsonPropertyName("isCommentsDisabled")]
        public bool IsCommentsDisabled { get; set; }
    }
}