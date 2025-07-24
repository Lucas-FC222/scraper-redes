namespace Services.Features.Facebook.Models
{
    public class ProcessDatasetResponse
    {
        /// <summary>
        /// Lista de posts processados do Facebook.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = new List<FacebookPost>();
    }
}
