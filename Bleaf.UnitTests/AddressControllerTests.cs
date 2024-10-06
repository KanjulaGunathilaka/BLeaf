using BLeaf.Controllers;
using BLeaf.Data;
using BLeaf.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bleaf.UnitTests
{
	[TestClass]
	public class AddressControllerTests
	{
		[TestMethod]
		public async Task WhenGetAddressIsCalled_CorrectAddressIsReturnedAsync()
		{

			var dbcontext = GetDbContext();
			//arrange
			var addressController = new AddressController(dbcontext);

			//act
			var actualAddress = await addressController.GetAddress(1);

			//assert
			Assert.AreEqual("Wellinton", actualAddress.Value.City);
		}

		

		private ApplicationDbContext GetDbContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new ApplicationDbContext(options);

				context.Users.Add(new User { UserId = 1,FullName="Jpe Blogs", BillingAddress = "", Email="", PasswordHash="", PhoneNumber="" });
				context.Addresses.Add(new Address { AddressId = 1, City = "Wellinton", UserId = 1, AddressLine1="", AddressLine2="", Country="", PhoneNumber="", State="", ZipCode="" });
				context.SaveChanges();
				return context;

		}

	}
}
