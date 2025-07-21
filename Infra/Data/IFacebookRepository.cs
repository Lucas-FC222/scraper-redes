using Core;

namespace Infra.Data
{
    public interface IFacebookRepository
    {
        Task SavePostsAsync(IEnumerable<FacebookPost> posts);
        Task<IEnumerable<FacebookPost>> GetAllPostsAsync();
        Task<FacebookPost?> GetPostByIdAsync(string id);
    }
} 