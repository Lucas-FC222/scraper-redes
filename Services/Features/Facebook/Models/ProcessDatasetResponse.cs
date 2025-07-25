namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Resposta para o processamento de um dataset do Facebook, contendo a lista de posts processados.
    /// </summary>
    public class ProcessDatasetResponse
    {
        /// <summary>
        /// Lista de posts processados do Facebook.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = [];
    }
}
