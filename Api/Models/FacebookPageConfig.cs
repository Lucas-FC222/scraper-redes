namespace Api.Models
{
    /// <summary>
    /// Representa a configuração de uma página do Facebook para scraping.
    /// </summary>
    public class FacebookPageConfig
    {
        /// <summary>
        /// Nome identificador da página do Facebook.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// URL da página do Facebook.
        /// </summary>
        public string PageUrl { get; set; } = string.Empty;
    }
}
