using Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IFacebookService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<string?> RunScraperAsync(string pageUrl, int maxPosts);
        Task<IEnumerable<Core.FacebookPost>> ProcessDatasetAsync(string datasetId);
        Task<IEnumerable<Core.FacebookPost>> GetAllPostsAsync();
        Task<Core.FacebookPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<Core.FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}