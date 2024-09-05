using BLeaf.Data;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLeaf.Controllers
{
	public class BLeafController : Controller
	{
		private readonly IItemRepository _itemRepository;
		private readonly ICategoryRepository _categoryRepository;

		public BLeafController(IItemRepository itemRepository, ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
			_itemRepository = itemRepository;
		}

		public IActionResult Index()
		{
			var items = _itemRepository.AllItems;
			var categories = _categoryRepository.AllCategories;

			return View(new BLeafViewModel(categories, items));
		}

		public IActionResult AboutUs()
		{
			return View();
		}

		public IActionResult BlogBothSidebar()
		{
			return View();
		}

		public IActionResult BlogDetailLeftSidebar()
		{
			return View();
		}

		public IActionResult BlogDetailRightSidebar()
		{
			return View();
		}

		public IActionResult BlogGrid2()
		{
			return View();
		}

		public IActionResult BlogGrid3()
		{
			return View();
		}

		public IActionResult BlogGrid3Masonry()
		{
			return View();
		}

		public IActionResult BlogGrid4Masonry()
		{
			return View();
		}

		public IActionResult BlogGridLeftSidebar()
		{
			return View();
		}

		public IActionResult BlogGridRightSidebar()
		{
			return View();
		}

		public IActionResult BlogList()
		{
			return View();
		}

		public IActionResult BlogListLeftSidebar()
		{
			return View();
		}

		public IActionResult BlogListRightSidebar()
		{
			return View();
		}

		public IActionResult BlogOpenGutenberg()
		{
			return View();
		}

		public IActionResult BlogStandard()
		{
			return View();
		}

		public IActionResult BlogWideGridSidebar()
		{
			return View();
		}

		public IActionResult BlogWideListSidebar()
		{
			return View();
		}

		public IActionResult CommingSoon()
		{
			return View();
		}

		public IActionResult ContactUs()
		{
			return View();
		}
		public IActionResult Error404()
		{
			return View();
		}
		public IActionResult FAQ()
		{
			return View();
		}
		public IActionResult OurMenu1()
		{
			var categories = _categoryRepository.AllCategories;
			var items = _itemRepository.AllItems;

			var viewModel = new BLeafViewModel
			{
				Categories = categories,
				Items = items
			};

			return View(viewModel);
		}
		public IActionResult OurMenu2()
		{
			return View();
		}
		public IActionResult OurMenu3()
		{
            var categories = _categoryRepository.AllCategories;
            var items = _itemRepository.AllItems;

            var viewModel = new BLeafViewModel
            {
                Categories = categories,
                Items = items
            };

            return View(viewModel);
        }
		public IActionResult OurMenu4()
		{
			return View();
		}
		public IActionResult OurMenu5()
		{
			return View();
		}
		public IActionResult ProductDetail()
		{
			return View();
		}
		public IActionResult ServiceDetail()
		{
			return View();
		}
		public IActionResult Services()
		{
			return View();
		}
		public IActionResult ShopCart()
		{
			return View();
		}
		public IActionResult ShopCheckout()
		{
			return View();
		}
		public IActionResult ShopStyle1()
		{
			return View();
		}
		public IActionResult ShopStyle2()
		{
			return View();
		}
		public IActionResult ShopWhislist()
		{
			return View();
		}
		public IActionResult Team()
		{
			return View();
		}
		public IActionResult TeamDetail()
		{
			return View();
		}
		public IActionResult Testmonial()
		{
			return View();
		}
		public IActionResult UnderMaintenance()
		{
			return View();
		}

        public IActionResult AjaxBlogGrid2()
        {
            return View();
        }

        public IActionResult AjaxBlogGrid3()
        {
            return View();
        }

        public IActionResult AjaxBlogList()
        {
            return View();
        }

        public IActionResult AjaxBlogListSidebar()
        {
            return View();
        }

        public IActionResult AjaxTestimonial()
        {
            return View();
        }
    }
}
