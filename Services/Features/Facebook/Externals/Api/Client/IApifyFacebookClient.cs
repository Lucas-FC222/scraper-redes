using Services.Features.Facebook.Models;
using Shared.Domain.Models;

namespace Services.Features.Facebook.Externals.Api.Client
{
    public interface IApifyFacebookClient
    {
        Task<Result<string>> RunFacebookScraperAsync(string pageUrl, int maxPosts);
        Task<Result<FacebookData>> ProcessFacebookDatasetAsync(string datasetId);
    }
}
