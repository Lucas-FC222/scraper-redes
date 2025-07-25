using System.Text.Json.Serialization;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Representa um usu�rio marcado (tagged) em um post do Instagram retornado pela API Apify.
    /// </summary>
    public class ApifyInstagramTaggedUser
    {
        /// <summary>
        /// Nome completo do usu�rio marcado.
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Identificador �nico do usu�rio marcado.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Indica se o usu�rio marcado � verificado.
        /// </summary>
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }
        /// <summary>
        /// URL da foto de perfil do usu�rio marcado.
        /// </summary>
        [JsonPropertyName("profile_pic_url")]
        public string ProfilePicUrl { get; set; } = string.Empty;
        /// <summary>
        /// Nome de usu�rio (username) do usu�rio marcado.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
}