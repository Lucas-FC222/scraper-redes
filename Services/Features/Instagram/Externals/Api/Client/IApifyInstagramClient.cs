using Services.Features.Instagram.Models;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Externals.Api.Client
{
    /// <summary>
    /// Interface responsável por operações de scraping e processamento de dados do Instagram via API Apify.
    /// </summary>
    public interface IApifyInstagramClient
    {
        /// <summary>
        /// Inicia o scraper do Instagram para um usuário específico.
        /// </summary>
        /// <param name="username">Nome de usuário do Instagram.</param>
        /// <param name="limit">Número máximo de posts a coletar.</param>
        /// <returns>Resultado contendo o RunId do scraper ou erro.</returns>
        Task<Result<string>> RunInstagramScraperAsync(string username, int limit);

        /// <summary>
        /// Processa o dataset do Instagram retornado pela Apify.
        /// </summary>
        /// <param name="datasetId">ID do dataset.</param>
        /// <returns>Resultado contendo os dados do Instagram processados ou erro.</returns>
        Task<Result<InstagramData>> ProcessInstagramDatasetAsync(string datasetId);
    }
}
