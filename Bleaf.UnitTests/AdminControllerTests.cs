using BLeaf.Controllers;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bleaf.UnitTests
{
    [TestClass]
    public class AdminControllerTests
    {
        private Mock<ICategoryRepository> fakeCategoryRepository;
        private Mock<IItemRepository> fakeItemRepository;
        private Mock<IUserRepository> fakeUserRepository;
        private Mock<IOrderRepository> fakeOrderRepository;
        private Mock<UserManager<IdentityUser>> mockUserManager;

        [TestInitialize]
        public void Setup()
        {
            fakeCategoryRepository = new Mock<ICategoryRepository>();
            fakeItemRepository = new Mock<IItemRepository>();
            fakeUserRepository = new Mock<IUserRepository>();
            fakeOrderRepository = new Mock<IOrderRepository>();
            mockUserManager = GetMockUserManager();
        }

        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }

        [TestMethod]
        public void WhenManagedItemsCalled_CategoriesAreLoaded()
        {
            fakeCategoryRepository.Setup(x => x.AllCategories).Returns(new List<Category>()
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            });

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object, fakeOrderRepository.Object);

            var view = adminController.ManageItems() as ViewResult;

            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(2, actualModel.Categories.Count());
        }

        [TestMethod]
        public void WhenManagedItemsCalled_ItemsAreLoaded()
        {
            fakeItemRepository.Setup(x => x.AllItems).Returns(new List<Item>()
            {
                new Item { ItemId = 1, Name = "Item 1" },
                new Item { ItemId = 2, Name = "Item 2" }
            }.AsQueryable());

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object, fakeOrderRepository.Object);

            var view = adminController.ManageItems() as ViewResult;

            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(2, actualModel.Items.Count());
        }

        [TestMethod]
        public void WhenManagedItemsCalled_NoCategories_ReturnsEmptyList()
        {
            fakeCategoryRepository.Setup(x => x.AllCategories).Returns(new List<Category>());

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object, fakeOrderRepository.Object);

            var view = adminController.ManageItems() as ViewResult;

            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(0, actualModel.Categories.Count());
        }

        [TestMethod]
        public void WhenManagedItemsCalled_NoItems_ReturnsEmptyList()
        {
            List<Item> items = new List<Item>();
            fakeItemRepository.Setup(x => x.AllItems).Returns(items.AsQueryable());

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object, fakeOrderRepository.Object);

            var view = adminController.ManageItems() as ViewResult;

            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(0, actualModel.Items.Count());
        }
    }
}