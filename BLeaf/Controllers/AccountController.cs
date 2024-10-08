using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BLeaf.ViewModels;
using BLeaf.Models;
using BLeaf.Data;

namespace BLeaf.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return PartialView("BLeaf/_Login", new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("BLeaf/_Login", new LoginViewModel());
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("AdminPanel", "Admin");
                }
                return RedirectToAction("Index", "BLeaf");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "BLeaf");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register", new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                return View("Register", model);
            }

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // Create and save the custom User model
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = identityUser.PasswordHash,
                    Role = "Customer",
                    //BillingAddress = model.BillingAddress
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "BLeaf");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Register", model);
        }
    }
}