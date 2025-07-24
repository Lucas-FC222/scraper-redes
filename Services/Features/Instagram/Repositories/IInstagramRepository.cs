using Services.Features.Instagram.Models;

namespace Services.Features.Instagram.Repositories
{
    public interface IInstagramRepository
    {
        Task SavePostsAsync(IEnumerable<InstagramPost> posts);
        Task SaveCommentsAsync(IEnumerable<InstagramComment> comments);
        Task SaveHashtagsAsync(IEnumerable<InstagramHashtag> hashtags);
        Task SaveMentionsAsync(IEnumerable<InstagramMention> mentions);
        Task<IEnumerable<InstagramPost>> GetAllPostsAsync();
        Task<InstagramPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<InstagramPost>> GetPostsByIdsAsync(IEnumerable<string> ids);
        Task<IEnumerable<InstagramComment>> GetCommentsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramHashtag>> GetHashtagsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramMention>> GetMentionsByPostIdAsync(string postId);
        Task<IEnumerable<InstagramPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
} 