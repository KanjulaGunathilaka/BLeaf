using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLeaf.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;

namespace BLeaf.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;

        public AdminController(ICategoryRepository categoryRepository, IItemRepository itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        // Action to manage items
        public IActionResult ManageItems()
        {
            var categories = _categoryRepository.AllCategories;
            var items = _itemRepository.AllItems;
            var adminViewModel = new BLeafViewModel(categories, items);
            return View(adminViewModel);
        }

        // Action to manage categories
        public IActionResult ManageCategories()
        {
            var categories = _categoryRepository.AllCategories;
            var adminViewModel = new BLeafViewModel { Categories = categories };
            return View(adminViewModel);
        }

        //// GET: Admin/Reviews
        //public async Task<IActionResult> Reviews()
        //      {
        //          ViewBag.Items = await _context.Items.ToListAsync();
        //          return View(await _context.Reviews.Include(r => r.Item).ToListAsync());
        //      }

        //      // GET: Admin/Users
        //      public async Task<IActionResult> Users()
        //      {
        //          return View(await _context.Users.ToListAsync());
        //      }

        //      // GET: Admin/Addresses
        //      public async Task<IActionResult> Addresses()
        //      {
        //          ViewBag.Users = await _context.Users.ToListAsync();
        //          return View(await _context.Addresses.Include(a => a.User).ToListAsync());
        //      }

        //      // GET: Admin/Orders
        //      public async Task<IActionResult> Orders()
        //      {
        //          ViewBag.Users = await _context.Users.ToListAsync();
        //          ViewBag.Addresses = await _context.Addresses.ToListAsync();
        //          return View(await _context.Orders.Include(o => o.User).Include(o => o.Address).ToListAsync());
        //      }

        //      // GET: Admin/OrderDetails
        //      public async Task<IActionResult> OrderDetails()
        //      {
        //          ViewBag.Orders = await _context.Orders.ToListAsync();
        //          ViewBag.Items = await _context.Items.ToListAsync();
        //          return View(await _context.OrderDetails.Include(od => od.Order).Include(od => od.Item).ToListAsync());
        //      }

        //      // GET: Admin/ShoppingCartItems
        //      public async Task<IActionResult> ShoppingCartItems()
        //      {
        //          ViewBag.Users = await _context.Users.ToListAsync();
        //          ViewBag.Items = await _context.Items.ToListAsync();
        //          return View(await _context.ShoppingCartItems.Include(sci => sci.User).Include(sci => sci.Item).ToListAsync());
        //      }

        //      // GET: Admin/Discounts
        //      public async Task<IActionResult> Discounts()
        //      {
        //          return View(await _context.Discounts.ToListAsync());
        //      }

        //      // GET: Admin/Reservations
        //      public async Task<IActionResult> Reservations()
        //      {
        //          ViewBag.Users = await _context.Users.ToListAsync();
        //          return View(await _context.Reservations.Include(r => r.User).ToListAsync());
        //      }

        // GET: Admin/AdminPanel
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}