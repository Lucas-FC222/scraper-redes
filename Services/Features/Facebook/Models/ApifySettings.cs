namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa as configurações necessárias para integração com a API Apify para Facebook.
    /// </summary>
    public class ApifySettings
    {
        /// <summary>
        /// Identificador do ator (actor) no Apify.
        /// </summary>
        public string ActorId { get; set; } = string.Empty;
        /// <summary>
        /// Token de acesso à API do Apify.
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;
        /// <summary>
        /// Segredo utilizado para validação de webhooks do Apify.
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;
    }
} 