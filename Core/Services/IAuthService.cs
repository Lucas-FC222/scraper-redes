namespace Shared.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AppUser?> GetUserByEmailAsync(string email);
        string GenerateJwtToken(AppUser user);
    }
}
