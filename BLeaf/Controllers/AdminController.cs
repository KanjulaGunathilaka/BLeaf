using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;

namespace BLeaf.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;

        public AdminController(ICategoryRepository categoryRepository, IItemRepository itemRepository, IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
            _userRepository = userRepository;
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
        public IActionResult ManageUsers()
        {
            var users = _userRepository.AllUsers;
            var adminViewModel = new BLeafViewModel { Users = users };
            return View(adminViewModel);
        }

        // GET: Admin/AdminPanel
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}