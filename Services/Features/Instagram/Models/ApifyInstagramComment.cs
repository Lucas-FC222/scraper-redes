using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um comentário do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramComment
    {
        /// <summary>
        /// Identificador único do comentário.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Texto do comentário.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usuário do autor do comentário.
        /// </summary>
        [JsonPropertyName("ownerUsername")]
        public string OwnerUsername { get; set; } = string.Empty;
        /// <summary>
        /// URL da foto de perfil do autor do comentário.
        /// </summary>
        [JsonPropertyName("ownerProfilePicUrl")]
        public string OwnerProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp do comentário.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de respostas ao comentário.
        /// </summary>
        [JsonPropertyName("repliesCount")]
        public int RepliesCount { get; set; }
        /// <summary>
        /// Lista de respostas ao comentário (pode conter objetos variados).
        /// </summary>
        [JsonPropertyName("replies")]
        public List<object> Replies { get; set; } = new();
        /// <summary>
        /// Quantidade de curtidas no comentário.
        /// </summary>
        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; }
        /// <summary>
        /// Informações do autor do comentário.
        /// </summary>
        [JsonPropertyName("owner")]
        public ApifyInstagramCommentOwner? Owner { get; set; }
    }
    
    /// <summary>
    /// Representa o autor de um comentário do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramCommentOwner
    {
        /// <summary>
        /// Identificador único do autor do comentário.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Indica se o autor é verificado.
        /// </summary>
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }
        /// <summary>
        /// URL da foto de perfil do autor.
        /// </summary>
        [JsonPropertyName("profile_pic_url")]
        public string ProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usuário do autor.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
} 