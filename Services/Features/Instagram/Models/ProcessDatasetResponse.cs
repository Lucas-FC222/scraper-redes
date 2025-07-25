namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Resposta para o processamento de um dataset do Instagram, contendo a lista de posts processados.
    /// </summary>
    public class ProcessDatasetResponse
    {
        /// <summary>
        /// Lista de posts processados do Instagram.
        /// </summary>
        public IEnumerable<InstagramPost> Posts { get; set; } = new List<InstagramPost>();
    }
}
