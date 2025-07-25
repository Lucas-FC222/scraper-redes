using Shared.Domain.Models;

namespace Services.Features.Posts.Externals.Client
{
    /// <summary>
    /// Interface responsável por classificar o conteúdo de posts utilizando a API Groq.
    /// </summary>
    public interface IGroqApiClient
    {
        /// <summary>
        /// Classifica o texto de um post em um tema específico.
        /// </summary>
        /// <param name="text">Texto do post a ser classificado.</param>
        /// <returns>Resultado contendo o tema classificado ou erro.</returns>
        Task<Result<string>> ClassifyPostAsync(string text);
    }
}
