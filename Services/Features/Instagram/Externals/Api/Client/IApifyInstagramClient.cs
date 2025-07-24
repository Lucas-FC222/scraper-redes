using Services.Features.Instagram.Models;
using Shared.Domain.Models;

namespace Services.Features.Instagram.Externals.Api.Client
{
    public interface IApifyInstagramClient
    {
        Task<Result<string>> RunInstagramScraperAsync(string username, int limit);
        Task<Result<InstagramData>> ProcessInstagramDatasetAsync(string datasetId);
    }
}
