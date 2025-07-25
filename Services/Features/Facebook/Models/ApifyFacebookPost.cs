using System.Text.Json.Serialization;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa um post do Facebook retornado pela API Apify, incluindo informações detalhadas do post.
    /// </summary>
    public class ApifyFacebookPost
    {
        /// <summary>
        /// Identificador único do post no Facebook.
        /// </summary>
        [JsonPropertyName("post_id")]
        public string PostId { get; set; } = string.Empty;
        /// <summary>
        /// URL do post no Facebook.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Mensagem/texto do post.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp do post (Unix time).
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        /// <summary>
        /// Quantidade de comentários no post.
        /// </summary>
        [JsonPropertyName("comments_count")]
        public int CommentsCount { get; set; }
        /// <summary>
        /// Quantidade de reações no post.
        /// </summary>
        [JsonPropertyName("reactions_count")]
        public int ReactionsCount { get; set; }
        /// <summary>
        /// Informações do autor do post.
        /// </summary>
        [JsonPropertyName("author")]
        public ApifyFacebookAuthor? Author { get; set; }
        /// <summary>
        /// Imagem associada ao post (pode ser um objeto JSON).
        /// </summary>
        [JsonPropertyName("image")]
        public object? Image { get; set; }
        /// <summary>
        /// Vídeo associado ao post (pode ser um objeto JSON).
        /// </summary>
        [JsonPropertyName("video")]
        public object? Video { get; set; }
        /// <summary>
        /// URL de post anexado (pode ser um objeto JSON).
        /// </summary>
        [JsonPropertyName("attached_post_url")]
        public object? AttachedPostUrl { get; set; }
    }
    
    /// <summary>
    /// Representa o autor de um post do Facebook retornado pela API Apify.
    /// </summary>
    public class ApifyFacebookAuthor
    {
        /// <summary>
        /// Identificador único do autor.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Nome do autor do post.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// URL do perfil do autor.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// URL da foto de perfil do autor.
        /// </summary>
        [JsonPropertyName("profile_picture_url")]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
} 