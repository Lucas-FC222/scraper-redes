namespace Shared.Services
{
    public interface IInstagramService
    {
        Task<string?> RunScraperAsync(string username, int limit);
        Task<IEnumerable<InstagramPost>> ProcessDatasetAsync(string datasetId);
        Task<IEnumerable<InstagramPost>> GetAllPostsAsync();
        Task<InstagramPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramHashtag>> GetHashtagsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
}
