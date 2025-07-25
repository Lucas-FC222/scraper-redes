namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para a execução do scraper do Instagram, contendo o identificador da execução.
    /// </summary>
    public class RunScraperResponse
    {
        /// <summary>
        /// Identificador da execução (run) do scraper no Apify ou serviço equivalente.
        /// </summary>
        public string RunId { get; set; } = string.Empty;
    }
}
