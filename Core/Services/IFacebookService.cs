namespace Shared.Services
{
    public interface IFacebookService
    {
        Task<string?> RunScraperAsync(string pageUrl, int maxPosts);
        Task<IEnumerable<FacebookPost>> ProcessDatasetAsync(string datasetId);
        Task<IEnumerable<FacebookPost>> GetAllPostsAsync();
        Task<FacebookPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}