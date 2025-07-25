namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa um conjunto de dados do Facebook contendo uma coleção de posts coletados.
    /// </summary>
    public class FacebookData
    {
        /// <summary>
        /// Coleção de posts do Facebook associados a este conjunto de dados.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = [];
    }
} 