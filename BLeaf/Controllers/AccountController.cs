using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BLeaf.ViewModels;
using BLeaf.Data;
using BLeaf.Services;

namespace BLeaf.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext context, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid login attempt." });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    HttpContext.Session.SetString("UserSessionID", user.Email);
                    return Json(new { success = true, redirectUrl = Url.Action("AdminPanel", "Admin") });
                }
                return Json(new { success = true, redirectUrl = Url.Action("Index", "BLeaf") });
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return Json(new { success = false, message = "Invalid login attempt." });
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
                return Json(new { success = false, message = "Invalid registration attempt." });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "A user with this email already exists." });
            }

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // Assign the default role (Customer) to the new user
                await _userManager.AddToRoleAsync(identityUser, "Customer");

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

                // Send a welcome email
                try
                {
                    var subject = "Welcome to Our Restaurant!";
                var plainTextContent = $"Dear {model.FullName},\n\nThank you for registering with our restaurant. We are excited to have you on board!\n\nBest Regards,\nYour Restaurant Name";
                var htmlContent = $@"
                    <html>
                    <body>
                        <h1>Welcome to Our Restaurant, {model.FullName}!</h1>
                        <p>Thank you for registering with our restaurant. We are excited to have you on board!</p>
                        <p>Here are some details about our restaurant:</p>
                        <ul>
                            <li><strong>Location:</strong> 408 Hutt Road, Alicetown, Lower Hutt 5010</li>
                            <li><strong>Opening Hours Weekday:</strong> 07:00 AM - 08:00 PM, Monday to Friday</li>
                            <li><strong>Opening Hours Weekend:</strong> 08:00 AM - 08:00 PM, Saturday and Sunday</li>
                            <li><strong>Contact:</strong> 022 522 0400</li>
                        </ul>
                        <p>As a registered member, you will enjoy exclusive discounts and special offers. Stay tuned for our latest updates and promotions!</p>
                        <p>If you have any questions or need assistance, feel free to reach out to us at bleafcaf@gmail.com.</p>
                        <p>We look forward to serving you!</p>
                        <p>Best Regards,<br>BLeaf Cafe</p>
                    </body>
                    </html>";

                await _emailSender.SendEmailAsync(model.Email, subject, plainTextContent, htmlContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
                return Json(new { success = true, redirectUrl = Url.Action("Index", "BLeaf") });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, message = errors });
        }
    }
}