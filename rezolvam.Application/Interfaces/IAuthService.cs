using Microsoft.AspNetCore.Identity;

namespace Rezolvam.Application.Interfaces;
public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<IdentityResult> RegisterAsync(string email, string password, string fullName);
}
