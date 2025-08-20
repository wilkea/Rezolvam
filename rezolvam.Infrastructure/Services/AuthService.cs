using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using rezolvam.Domain.Common;
using Rezolvam.Application.Interfaces;

namespace rezolvam.Infrastructure.Services
{
    public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<SignInResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
    }

    public async Task<IdentityResult> RegisterAsync(string email, string password, string fullName)
    {
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.FullName ?? "")
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
    }
    }

}
