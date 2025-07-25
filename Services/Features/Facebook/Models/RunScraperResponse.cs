namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Resposta para a execução do scraper do Facebook, contendo o identificador da execução.
    /// </summary>
    public class RunScraperResponse
    {
        /// <summary>
        /// Identificador da execução (run) do scraper no Apify ou serviço equivalente.
        /// </summary>
        public string RunId { get; set; } = string.Empty;
    }
}
