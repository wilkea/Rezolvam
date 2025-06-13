using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using rezolvam.Domain.Common;

namespace Rezolvam.Application.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password);
        Task<IdentityResult> RegisterAsync(string email, string password, string fullName);
        Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user);
    }
}
