using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para executar o scraper em um perfil do Instagram.
    /// </summary>
    public class RunScraperRequest : IRequest<Result<RunScraperResponse>>
    {
        /// <summary>
        /// Nome de usuário do perfil do Instagram a ser processado pelo scraper.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Quantidade máxima de posts a serem coletados pelo scraper.
        /// </summary>
        public int Limit { get; set; }
    }
}
