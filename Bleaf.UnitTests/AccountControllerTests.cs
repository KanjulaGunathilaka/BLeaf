﻿using BLeaf.Controllers;
using BLeaf.Models;
using BLeaf.Models.IRepository;
using BLeaf.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;

namespace Bleaf.UnitTests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }

        private Mock<SignInManager<IdentityUser>> GetMockSignInManager(Mock<UserManager<IdentityUser>> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            return new Mock<SignInManager<IdentityUser>>(userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }

        [TestMethod]
        public void WhenManagedItemsCalled_CategoriesAreLoaded()
        {
            // Arrange
            var fakeCategoryRepository = new Mock<ICategoryRepository>();
            var fakeItemRepository = new Mock<IItemRepository>();
            var fakeUserRepository = new Mock<IUserRepository>();
            var mockUserManager = GetMockUserManager();
            var mockSignInManager = GetMockSignInManager(mockUserManager);

            fakeCategoryRepository.Setup(x => x.AllCategories).Returns(new List<Category>()
            {
                new Category { CategoryId = 1, Name = "Category 1" },
                new Category { CategoryId = 2, Name = "Category 2" }
            });

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object);

            // Act
            var view = adminController.ManageItems() as ViewResult;

            // Assert
            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(2, actualModel.Categories.Count());
        }

        [TestMethod]
        public void WhenManagedItemsCalled_ItemsAreLoaded()
        {
            // Arrange
            var fakeCategoryRepository = new Mock<ICategoryRepository>();
            var fakeItemRepository = new Mock<IItemRepository>();
            var fakeUserRepository = new Mock<IUserRepository>();
            var mockUserManager = GetMockUserManager();
            var mockSignInManager = GetMockSignInManager(mockUserManager);

            fakeItemRepository.Setup(x => x.AllItems).Returns(new List<Item>()
            {
                new Item { ItemId = 1, Name = "Item 1" },
                new Item { ItemId = 2, Name = "Item 2" }
            }.AsQueryable());

            var adminController = new AdminController(fakeCategoryRepository.Object, fakeItemRepository.Object, fakeUserRepository.Object, mockUserManager.Object);

            // Act
            var view = adminController.ManageItems() as ViewResult;

            // Assert
            var actualModel = view.Model as BLeafViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(2, actualModel.Items.Count());
        }
    }
}