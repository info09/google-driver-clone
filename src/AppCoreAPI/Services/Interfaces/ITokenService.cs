using AppCoreAPI.Data.Entities;

namespace AppCoreAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser appUser);

        Task<string> GenerateRefreshToken();
    }
}
