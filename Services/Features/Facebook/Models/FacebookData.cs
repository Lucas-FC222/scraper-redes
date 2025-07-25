namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Representa um conjunto de dados do Facebook contendo uma cole��o de posts coletados.
    /// </summary>
    public class FacebookData
    {
        /// <summary>
        /// Cole��o de posts do Facebook associados a este conjunto de dados.
        /// </summary>
        public IEnumerable<FacebookPost> Posts { get; set; } = [];
    }
} 