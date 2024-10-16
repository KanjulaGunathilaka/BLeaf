using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using BLeaf.Models.Repository;

namespace BLeaf.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public AdminController(ICategoryRepository categoryRepository, IItemRepository itemRepository, IUserRepository userRepository, UserManager<IdentityUser> userManager, IOrderRepository orderRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        // Action to manage items
        public IActionResult ManageItems()
        {
            var categories = _categoryRepository.AllCategories;
            var items = _itemRepository.AllItems;
            var users = _userRepository.AllUsers;

            var adminViewModel = new BLeafViewModel(categories, items, users);
            return View(adminViewModel);
        }

        // Action to manage categories
        public IActionResult ManageCategories()
        {
            var categories = _categoryRepository.AllCategories;
            var adminViewModel = new BLeafViewModel { Categories = categories };
            return View(adminViewModel);
        }

        // Action to manage users
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userRepository.AllUsers;
            var roles = new List<string> { "Customer", "Admin" };
            var adminViewModel = new BLeafViewModel { Users = users, Roles = roles };
            return View(adminViewModel);
        }

        public IActionResult ManageOrders()
        {
            var orders = _orderRepository.GetAllAsync().Result;
            return View(orders);
        }

        // GET: Admin/AdminPanel
        public IActionResult AdminPanel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string email, string newRole)
        {
            var user = await _userRepository.FindUserByEmail(email);
            if (user != null)
            {
                user.Role = newRole;
                await _userRepository.UpdateUser(user);
            }

            // Update the role in the ASP.NET Identity system
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
                await _userManager.AddToRoleAsync(identityUser, newRole);
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            // Delete from custom Users table
            var user = await _userRepository.FindUserByEmail(email);
            if (user != null)
            {
                await _userRepository.DeleteUser(user.UserId);
            }

            // Delete from ASP.NET Identity table
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);
            }

            return RedirectToAction("ManageUsers");
        }
    }
}