using MediatR;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Models
{
    public class ProcessDatasetRequest : IRequest<Result<ProcessDatasetResponse>>
    {
        public string DatasetId { get; set; } = string.Empty;
    }
}
