using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um coment�rio do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramComment
    {
        /// <summary>
        /// Identificador �nico do coment�rio.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Texto do coment�rio.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usu�rio do autor do coment�rio.
        /// </summary>
        [JsonPropertyName("ownerUsername")]
        public string OwnerUsername { get; set; } = string.Empty;
        /// <summary>
        /// URL da foto de perfil do autor do coment�rio.
        /// </summary>
        [JsonPropertyName("ownerProfilePicUrl")]
        public string OwnerProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp do coment�rio.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade de respostas ao coment�rio.
        /// </summary>
        [JsonPropertyName("repliesCount")]
        public int RepliesCount { get; set; }
        /// <summary>
        /// Lista de respostas ao coment�rio (pode conter objetos variados).
        /// </summary>
        [JsonPropertyName("replies")]
        public List<object> Replies { get; set; } = new();
        /// <summary>
        /// Quantidade de curtidas no coment�rio.
        /// </summary>
        [JsonPropertyName("likesCount")]
        public int LikesCount { get; set; }
        /// <summary>
        /// Informa��es do autor do coment�rio.
        /// </summary>
        [JsonPropertyName("owner")]
        public ApifyInstagramCommentOwner? Owner { get; set; }
    }
    
    /// <summary>
    /// Representa o autor de um coment�rio do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramCommentOwner
    {
        /// <summary>
        /// Identificador �nico do autor do coment�rio.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Indica se o autor � verificado.
        /// </summary>
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }
        /// <summary>
        /// URL da foto de perfil do autor.
        /// </summary>
        [JsonPropertyName("profile_pic_url")]
        public string ProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usu�rio do autor.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
} 