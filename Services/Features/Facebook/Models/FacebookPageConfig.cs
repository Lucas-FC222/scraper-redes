namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa a configuração de uma página do Facebook para operações de scraping.
    /// </summary>
    public class FacebookPageConfig
    {
        /// <summary>
        /// Nome da página do Facebook.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// URL da página do Facebook.
        /// </summary>
        public string PageUrl { get; set; } = string.Empty;
    }
}
