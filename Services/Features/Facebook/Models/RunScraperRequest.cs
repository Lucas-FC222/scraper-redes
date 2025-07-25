using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Requisição para executar o scraper em uma página do Facebook.
    /// </summary>
    public class RunScraperRequest : IRequest<Result<RunScraperResponse>>
    {
        /// <summary>
        /// URL da página do Facebook a ser processada pelo scraper.
        /// </summary>
        public string PageUrl { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade máxima de posts a serem coletados pelo scraper.
        /// </summary>
        public int MaxPosts { get; set; }
    }
}
