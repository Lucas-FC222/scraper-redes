using Core.Models;

namespace Core.Repositories
{
    public interface IFacebookRepository
    {
        Task SavePostsAsync(IEnumerable<FacebookPost> posts);
        Task<IEnumerable<FacebookPost>> GetAllPostsAsync();
        Task<FacebookPost?> GetPostByIdAsync(string id);
        Task<IEnumerable<FacebookPost>> SearchPostsByKeywordsAsync(IEnumerable<string> keywords);
    }
} 