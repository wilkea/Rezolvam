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

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Login cu Identity cookie, fără JWT
        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe = false)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return SignInResult.Failed;

            // Aici setezi direct cookie-ul de Identity (fără token)
            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
            return result;
        }

        public async Task<IdentityResult> RegisterAsync(string email, string password, string fullName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return result;

            var roleResult = await _userManager.AddToRoleAsync(user, "user");
            // Dacă adăugarea în rol eșuează, returnezi roleResult, altfel returnezi result-ul de create
            return roleResult.Succeeded ? result : roleResult;
        }
        
        // Logout
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public Task<IEnumerable<ClaimsIdentity>> CreateIdentityAsync(object user)
        {
            throw new NotImplementedException();
        }
    }
}
