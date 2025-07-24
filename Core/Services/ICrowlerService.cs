namespace Shared.Services
{
    public interface ICrowlerService
    {
        Task<string> RunInstagramScraperAsync(string username, int limit);
        Task<string> RunFacebookScraperAsync(string pageUrl, int maxPosts);
        Task<InstagramData?> ProcessInstagramDatasetAsync(string datasetId);
        Task<FacebookData?> ProcessFacebookDatasetAsync(string datasetId);
    }
}
