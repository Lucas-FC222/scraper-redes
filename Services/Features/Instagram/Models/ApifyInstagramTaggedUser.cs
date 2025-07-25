using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um usuário marcado (tagged) em um post do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramTaggedUser
    {
        /// <summary>
        /// Nome completo do usuário marcado.
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Identificador único do usuário marcado.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Indica se o usuário marcado é verificado.
        /// </summary>
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }
        /// <summary>
        /// URL da foto de perfil do usuário marcado.
        /// </summary>
        [JsonPropertyName("profile_pic_url")]
        public string ProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usuário (username) do usuário marcado.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
}