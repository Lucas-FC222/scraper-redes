using Services.Features.Facebook.Models;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Externals.Api.Client
{
    /// <summary>
    /// Interface responsável por operações de scraping e processamento de dados do Facebook via API Apify.
    /// </summary>
    public interface IApifyFacebookClient
    {
        /// <summary>
        /// Inicia o scraper do Facebook para uma página específica.
        /// </summary>
        /// <param name="pageUrl">URL da página do Facebook.</param>
        /// <param name="maxPosts">Número máximo de posts a coletar.</param>
        /// <returns>Resultado contendo o RunId do scraper ou erro.</returns>
        Task<Result<string>> RunFacebookScraperAsync(string pageUrl, int maxPosts);

        /// <summary>
        /// Processa o dataset do Facebook retornado pela Apify.
        /// </summary>
        /// <param name="datasetId">ID do dataset.</param>
        /// <returns>Resultado contendo os dados do Facebook processados ou erro.</returns>
        Task<Result<FacebookData>> ProcessFacebookDatasetAsync(string datasetId);
    }
}
