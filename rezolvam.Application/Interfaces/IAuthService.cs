using Microsoft.AspNetCore.Identity;
using rezolvam.Domain.Common;

namespace Rezolvam.Application.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe = false);
        Task<IdentityResult> RegisterAsync(string email, string password, string fullName);
        Task LogoutAsync();
    }
}
