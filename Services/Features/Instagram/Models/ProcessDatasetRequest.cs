using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Models
{
    /// <summary>
    /// Requisição para processar um dataset do Instagram, identificando-o pelo DatasetId.
    /// </summary>
    public class ProcessDatasetRequest : IRequest<Result<ProcessDatasetResponse>>
    {
        /// <summary>
        /// Identificador do dataset a ser processado.
        /// </summary>
        public string DatasetId { get; set; } = string.Empty;
    }
}
