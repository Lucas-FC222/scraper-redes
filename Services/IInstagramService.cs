namespace Services
{
    public interface IInstagramService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<string?> RunScraperAsync(string username, int limit);
        Task<IEnumerable<Core.InstagramPost>> ProcessDatasetAsync(string datasetId);
        Task<IEnumerable<Core.InstagramPost>> GetAllPostsAsync();
        Task<Core.InstagramPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<Core.InstagramComment>> GetCommentsByPostIdAsync(string postId);
        Task<IEnumerable<Core.InstagramHashtag>> GetHashtagsByPostIdAsync(string postId);
        Task<IEnumerable<Core.InstagramMention>> GetMentionsByPostIdAsync(string postId);
    }
}
