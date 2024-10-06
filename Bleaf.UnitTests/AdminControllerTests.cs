using BLeaf.Controllers;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bleaf.UnitTests
{
	[TestClass]
	public class AdminControllerTests
	{
		[TestMethod]
		public void WhenMangedItemsCalled_CategoriesAreLoaded()
		{
			//arrange
			var fakeCategoryRepository = new Moq.Mock<ICategoryRepository>();
			var fakeItemRepository = new Moq.Mock<IItemRepository>();
			fakeCategoryRepository.Setup(x => x.AllCategories).Returns(new List<Category>()
			{
				new Category { CategoryId = 1, Name = "Category 1" },
				new Category { CategoryId = 2, Name = "Category 2" }
			});

			var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object);
			//act

			var view = adminController.ManageItems() as ViewResult;
			//assert

			var actualModel = view.Model as BLeafViewModel;
			Assert.IsNotNull(actualModel);
			Assert.AreEqual(2,actualModel.Categories.Count());
		}


		[TestMethod]
		public void WhenMangedItemsCalled_ItemsAreLoaded()
		{
			//arrange
			var fakeCategoryRepository = new Moq.Mock<ICategoryRepository>();
			var fakeItemRepository = new Moq.Mock<IItemRepository>();
			fakeItemRepository.Setup(x=>x.AllItems).Returns(new List<Item>()
			{
				new Item { ItemId = 1, Name = "Item 1" },
				new Item { ItemId = 2, Name = "Item 2" }
			}.AsQueryable);

			var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object);
			//act

			var view = adminController.ManageItems() as ViewResult;
			//assert

			var actualModel = view.Model as BLeafViewModel;
			Assert.IsNotNull(actualModel);
			Assert.AreEqual(2, actualModel.Items.Count());
		}
	}
}