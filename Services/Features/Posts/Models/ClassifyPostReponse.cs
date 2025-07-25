namespace Services.Features.Posts.Models
{
    /// <summary>
    /// Representa a resposta da classificação de um post, contendo o tema classificado.
    /// </summary>
    public class ClassifyPostReponse
    {
        /// <summary>
        /// Tema classificado do post.
        /// </summary>
        public string Classification { get; set; } = string.Empty;
    }
}
