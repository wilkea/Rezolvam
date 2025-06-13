using AdminMVC.ViewModel.Auth;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.Queries.Report;
using rezolvam.Domain.Common;

namespace rezolvam.AdminMVC.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMediator mediator,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Găsește userul
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Încearcă autentificarea
            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Ai deja cookie-ul, deci redirect direct:
            return RedirectToAction("Profile");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile([FromQuery] PaginationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var query = new GetReportsForUserQuery
            {
                UserId = Guid.Parse(user.Id),
                IsAdmin = User.IsInRole("Admin"),
                Request = request
            };

            var result = await _mediator.Send(query);

            var model = new ProfileViewModel
            {
                Email = user.Email ?? "",
                FullName = user.FullName ,
                Roles = roles.ToList(),
                Reports = new PagedViewModel<ReportViewModel>
                {
                    Items = result.Items.Select(r => _mapper.Map<ReportViewModel>(r)).ToList(),
                    TotalCount = result.TotalCount,
                    PageIndex = result.PageIndex,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasNextPage = result.PageIndex < result.TotalPages,
                    HasPreviousPage = result.PageIndex > 1
                }
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, "User");
            // Poți autentifica automat sau trimite la Login
            // await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
        

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
