using Core.Models;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByIdAsync(Guid id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser?> CreateAsync(AppUser user);
        Task UpdateAsync(AppUser user);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
