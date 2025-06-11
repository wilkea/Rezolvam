using AdminMVC.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Domain.Common;

namespace AdminMVC.Controllers;

[Authorize(Roles = "Admin")]
public class HelperManagementController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HelperManagementController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var helpers = await _userManager.GetUsersInRoleAsync("Helper");
        return View(helpers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var helper = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName
        };
        var result = await _userManager.CreateAsync(helper, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(helper, "Helper");
            TempData["Success"] = "Helper account created";
            return RedirectToAction(nameof(Index));
        }
        foreach (var err in result.Errors)
            ModelState.AddModelError(string.Empty, err.Description);
        return View(model);
    }
}
