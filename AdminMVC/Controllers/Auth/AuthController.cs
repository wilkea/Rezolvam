using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AdminMVC.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rezolvam.Application.Interfaces;

namespace rezolvam.AdminMVC.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var token = await _authService.LoginAsync(model.Email, model.Password);
            if (token == null)
            {
                ModelState.AddModelError("", "Invalid login.");
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims;
            // Use Identity's application scheme instead of "Cookies"
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in with Identity's scheme
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterAsync(model.Email, model.Password, model.FullName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var user = User;
            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            var model = new ProfileViewModel
            {
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "",
                FullName = user.Identity?.Name ?? "",
                Roles = roles
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out using Identity's scheme
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login");
        }
    }

    
}