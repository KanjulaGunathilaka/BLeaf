using BLeaf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLeaf.Data
{
	public static class DbInitializer
	{
		public static async Task Seed(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
				var userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();
				var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

				context.Database.EnsureCreated();

				// Seed Roles
				string[] roleNames = { "Admin", "User" };
				foreach (var roleName in roleNames)
				{
					var normalizedRoleName = roleName.ToUpper();
					var roleExists = await roleManager.Roles.AnyAsync(r => r.NormalizedName == normalizedRoleName);
					if (!roleExists)
					{
						Console.WriteLine($"Creating role: {roleName}");
						await roleManager.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = normalizedRoleName });
					}
					else
					{
						Console.WriteLine($"Role already exists: {roleName}");
					}
				}

				// Seed Admin User
				var adminUser = await userManager.FindByEmailAsync("admin@bleaf.com");
				if (adminUser == null)
				{
					var user = new IdentityUser
					{
						UserName = "admin@bleaf.com",
						Email = "admin@bleaf.com",
						EmailConfirmed = true,
						NormalizedEmail = "admin@bleaf.com".ToUpper(),
						NormalizedUserName = "admin@bleaf.com".ToUpper()
					};

					var createUser = await userManager.CreateAsync(user, "Admin@123");
					if (createUser.Succeeded)
					{
						await userManager.AddToRoleAsync(user, "Admin");
					}
				}
				else
				{
					Console.WriteLine("Admin user already exists.");
				}

				// Seed Users
				if (!context.Users.Any())
				{
					var users = new User[]
					{
						new User{FullName="John Doe", Email="john@example.com", PhoneNumber="1234567890", BillingAddress="123 Main St", PasswordHash="hashedpassword", JoinedAt=DateTime.Now, Role="Customer"},
						new User{FullName="Admin User", Email="admin@bleaf.com", PhoneNumber="0987654321", BillingAddress="456 Admin St", PasswordHash="adminhashedpassword", JoinedAt=DateTime.Now, Role="Admin"}
					};

					foreach (var u in users)
					{
						context.Users.Add(u);
					}
					context.SaveChanges();
				}

				// Seed Addresses
				if (!context.Addresses.Any())
				{
					var addresses = new Address[]
					{
						new Address{UserId=1, AddressLine1="123 Main St", AddressLine2="", ZipCode="12345", City="Colombo", State="Western Province", Country="Sri Lanka", PhoneNumber="1234567890", IsPrimary=true},
						new Address{UserId=2, AddressLine1="456 Admin St", AddressLine2="", ZipCode="67890", City="Kandy", State="Central Province", Country="Sri Lanka", PhoneNumber="0987654321", IsPrimary=true}
					};

					foreach (var a in addresses)
					{
						context.Addresses.Add(a);
					}
					context.SaveChanges();
				}

				// Seed Categories
				if (!context.Categories.Any())
				{
					var categories = new Category[]
					{
						new Category{Name="Breakfast", Description="Start your day with a nutritious and delicious breakfast!"},
						new Category{Name="Lunch", Description="Hearty and satisfying meals to fuel your afternoon."},
						new Category{Name="Sri Lankan Dishes", Description="Authentic Sri Lankan cuisine with rich flavors and spices."},
						new Category{Name="Beverages", Description="Refreshing drinks to complement your meal."},
						new Category{Name="Snacks", Description="Tasty and convenient snacks for any time of the day."},
						new Category{Name="Dessert", Description="Indulgent and sweet treats to end your meal on a high note."},
						new Category{Name="Other", Description="A variety of other delightful options to explore."}
					};

					foreach (var c in categories)
					{
						context.Categories.Add(c);
					}
					context.SaveChanges();
				}

				// Fetch categories to use their IDs for items
				var breakfastCategory = context.Categories.First(c => c.Name == "Breakfast").CategoryId;
				var lunchCategory = context.Categories.First(c => c.Name == "Lunch").CategoryId;
				var sriLankanDishesCategory = context.Categories.First(c => c.Name == "Sri Lankan Dishes").CategoryId;
				var beveragesCategory = context.Categories.First(c => c.Name == "Beverages").CategoryId;
				var snacksCategory = context.Categories.First(c => c.Name == "Snacks").CategoryId;
				var dessertCategory = context.Categories.First(c => c.Name == "Dessert").CategoryId;
				var otherCategory = context.Categories.First(c => c.Name == "Other").CategoryId;

				// Seed Items
				if (!context.Items.Any())
				{
					var items = new Item[]
					{
						//Breakfast
						new Item { Name = "B'leaf Mini Break", Description = "A delightful mini breakfast platter featuring a selection of fresh fruits, nuts, and a small serving of yogurt, perfect for a light and nutritious start to your day.", Ingredients = "Fresh Fruits (Berries, Apple, Banana), Yogurt, Mixed Nuts, Honey", SpecialInformation = "Contains Dairy, Contains Nuts", Price = 12.00M, ImageUrl = "/bleaf/images/modal/bleaf_mini_break.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/bleaf_mini_break_thumb.png", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Avo & Tomato on Toast", Description = "A delicious and healthy breakfast option featuring creamy avocado and fresh tomatoes on toasted bread.", Ingredients = "Avocado, Tomato, Bread, Olive Oil, Salt, Pepper", SpecialInformation = "Vegan, Contains Gluten", Price = 10.00M, ImageUrl = "/bleaf/images/modal/avo_tomato_toast.png", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/avo_tomato_toast_thumb.png", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Egg Benedict", Description = "A classic breakfast dish featuring poached eggs and Canadian bacon on an English muffin, topped with rich hollandaise sauce.", Ingredients = "Eggs, Canadian Bacon, English Muffin, Butter, Lemon Juice, Salt, Pepper", SpecialInformation = "Contains Eggs, Contains Gluten, Contains Dairy", Price = 18.00M, ImageUrl = "/bleaf/images/modal/egg_benedict.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_benedict_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Egg Your Way on Toast", Description = "Customize your breakfast with eggs cooked to your preference, served on toasted bread.", Ingredients = "Eggs, Bread, Butter, Salt, Pepper", SpecialInformation = "Contains Eggs, Contains Gluten, Contains Dairy", Price = 10.00M, ImageUrl = "/bleaf/images/modal/egg_your_way_toast.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_your_way_toast_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Omelette with Toast", Description = "A fluffy omelette made with your choice of fillings, served with a side of toasted bread.", Ingredients = "Eggs, Bread, Butter, Salt, Pepper, Choice of Fillings (Cheese, Ham, Vegetables, etc.)", SpecialInformation = "Contains Eggs, Contains Gluten, Contains Dairy", Price = 18.00M, ImageUrl = "/bleaf/images/modal/omelette_with_toast.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/omelette_with_toast_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Creamy Mushroom on Toast", Description = "A savory and creamy mushroom mixture served on top of toasted bread, perfect for a hearty breakfast or brunch.", Ingredients = "Mushrooms, Bread, Cream, Garlic, Butter, Olive Oil, Salt, Pepper, Parsley", SpecialInformation = "Contains Dairy, Contains Gluten", Price = 16.00M, ImageUrl = "/bleaf/images/modal/creamy_mushroom_toast.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/creamy_mushroom_toast_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "Porridge with Fruits", Description = "A warm and hearty bowl of porridge topped with a variety of fresh fruits, perfect for a nutritious start to your day.", Ingredients = "Oats, Milk or Water, Fresh Fruits (Banana, Berries, Apple, etc.), Honey, Nuts (optional)", SpecialInformation = "Contains Gluten, Contains Dairy (if made with milk), Nuts (optional)", Price = 10.00M, ImageUrl = "/bleaf/images/modal/porridge_with_fruits.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/porridge_with_fruits_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						new Item { Name = "B'leaf Big Break", Description = "A hearty and wholesome breakfast platter featuring a variety of fresh fruits, nuts, yogurt, and a serving of granola, perfect for a fulfilling start to your day.", Ingredients = "Fresh Fruits (Berries, Apple, Banana, etc.), Yogurt, Mixed Nuts, Granola, Honey", SpecialInformation = "Contains Dairy, Contains Nuts, Contains Gluten (if granola contains gluten)", Price = 19.50M, ImageUrl = "/bleaf/images/modal/bleaf_big_break.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/bleaf_big_break_thumb.jpg", InStock = true, CategoryId = breakfastCategory, IsSpecial = false },
						//Lunch
						new Item { Name = "Angus Beef", Description = "A luxurious burger featuring a succulent Angus beef patty, caramelized onions, melted cheddar cheese, fresh lettuce, tomato, and a special house sauce, all served on a toasted sesame seed bun.", Ingredients = "Angus Beef Patty, Cheddar Cheese, Caramelized Onions, Lettuce, Tomato, Sesame Seed Bun, House Sauce, Pickles, Salt, Pepper", SpecialInformation = "Contains Gluten, Contains Dairy", Price = 18.00M, ImageUrl = "/bleaf/images/modal/angus_beef.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/angus_beef_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = true },
						new Item { Name = "Gourmet Chicken", Description = "A premium chicken burger featuring a grilled chicken breast, avocado, fresh lettuce, tomato, and a tangy aioli sauce, all served on a toasted brioche bun.", Ingredients = "Grilled Chicken Breast, Avocado, Lettuce, Tomato, Brioche Bun, Aioli Sauce, Salt, Pepper", SpecialInformation = "Contains Gluten, Contains Dairy (if aioli contains dairy)", Price = 18.00M, ImageUrl = "/bleaf/images/modal/gourmet_chicken.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/gourmet_chicken_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = true },
						new Item { Name = "Warm Chicken Grill Salad", Description = "A delicious and nutritious salad featuring warm grilled chicken, mixed greens, cherry tomatoes, cucumber, red onion, and a light vinaigrette.", Ingredients = "Grilled Chicken, Mixed Greens, Cherry Tomatoes, Cucumber, Red Onion, Vinaigrette (Olive Oil, Vinegar, Lemon Juice, Salt, Pepper)", SpecialInformation = "Contains Dairy (if vinaigrette contains dairy), Gluten-Free", Price = 18.00M, ImageUrl = "/bleaf/images/modal/warm_chicken_grill_salad.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/warm_chicken_grill_salad_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						new Item { Name = "Warm Lamb Grill Salad", Description = "A hearty and flavorful salad featuring warm grilled lamb, mixed greens, cherry tomatoes, cucumber, red onion, and a light vinaigrette.", Ingredients = "Grilled Lamb, Mixed Greens, Cherry Tomatoes, Cucumber, Red Onion, Vinaigrette (Olive Oil, Vinegar, Lemon Juice, Salt, Pepper)", SpecialInformation = "Contains Dairy (if vinaigrette contains dairy), Gluten-Free", Price = 18.00M, ImageUrl = "/bleaf/images/modal/warm_lamb_grill_salad.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/warm_lamb_grill_salad_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "Beef Nachos", Description = "A hearty plate of nachos topped with seasoned ground beef, melted cheese, jalapeños, sour cream, guacamole, and fresh salsa, perfect for sharing or enjoying as a meal.", Ingredients = "Tortilla Chips, Ground Beef, Cheddar Cheese, Jalapeños, Sour Cream, Guacamole, Salsa, Onions, Tomatoes, Spices", SpecialInformation = "Contains Dairy, Gluten-Free (if tortilla chips are gluten-free)", Price = 17.00M, ImageUrl = "/bleaf/images/modal/beef_nachos.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/beef_nachos_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						new Item { Name = "Veggie Nachos", Description = "A delicious and colorful plate of nachos topped with a variety of fresh vegetables, melted cheese, jalapeños, sour cream, guacamole, and fresh salsa, perfect for sharing or enjoying as a meal.", Ingredients = "Tortilla Chips, Cheddar Cheese, Bell Peppers, Black Beans, Corn, Jalapeños, Sour Cream, Guacamole, Salsa, Onions, Tomatoes, Spices", SpecialInformation = "Vegetarian, Contains Dairy, Gluten-Free (if tortilla chips are gluten-free)", Price = 17.00M, ImageUrl = "/bleaf/images/modal/veggie_nachos.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/veggie_nachos_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						new Item { Name = "Porterhouse Steak, Eggs, and Chips", Description = "A hearty and satisfying meal featuring a succulent porterhouse steak, two eggs cooked to your preference, and a generous serving of crispy chips.", Ingredients = "Porterhouse Steak, Eggs, Potatoes, Salt, Pepper, Cooking Oil, Butter (optional)", SpecialInformation = "Contains Eggs, Gluten-Free (if chips are gluten-free)", Price = 19.99M, ImageUrl = "/bleaf/images/modal/porterhouse_steak_eggs_chips.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/porterhouse_steak_eggs_chips_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						new Item { Name = "Ham and Cheese Toastie", Description = "A classic toastie featuring layers of succulent ham and melted cheese, grilled to perfection between slices of golden toasted bread.", Ingredients = "Ham, Cheese, Bread, Butter", SpecialInformation = "Contains Gluten, Contains Dairy, Contains Pork", Price = 5.99M, ImageUrl = "/bleaf/images/modal/ham_cheese_toastie.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/ham_cheese_toastie_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Tomato and Mozzarella Toastie", Description = "A delicious toastie featuring fresh tomato slices and melted mozzarella cheese, grilled to perfection between slices of golden toasted bread.", Ingredients = "Tomato, Mozzarella Cheese, Bread, Basil, Olive Oil, Butter", SpecialInformation = "Vegetarian, Contains Gluten, Contains Dairy", Price = 5.49M, ImageUrl = "/bleaf/images/modal/tomato_mozzarella_toastie.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/tomato_mozzarella_toastie_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						new Item { Name = "Chicken Pesto Toastie", Description = "A flavorful toastie featuring grilled chicken, pesto sauce, and melted cheese, grilled to perfection between slices of golden toasted bread.", Ingredients = "Grilled Chicken, Pesto Sauce, Cheese, Bread, Butter", SpecialInformation = "Contains Gluten, Contains Dairy, Contains Nuts (if pesto contains nuts)", Price = 6.49M, ImageUrl = "/bleaf/images/modal/chicken_pesto_toastie.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/chicken_pesto_toastie_thumb.jpg", InStock = true, CategoryId = lunchCategory, IsSpecial = false },
						//Sri Lankan
						new Item { Name = "Egg Kottu", Description = "A popular Sri Lankan street food dish made with chopped roti, stir-fried with eggs, vegetables, and aromatic spices, creating a flavorful and satisfying meal.", Ingredients = "Chopped Roti, Eggs, Carrots, Cabbage, Onions, Green Chilies, Soy Sauce, Garlic, Ginger, Curry Leaves, Spices", SpecialInformation = "Vegetarian, Contains Gluten, Contains Eggs, Contains Soy", Price = 19.50M, ImageUrl = "/bleaf/images/modal/egg_kottu.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_kottu_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = true },
						new Item { Name = "Chicken Kottu", Description = "A popular Sri Lankan street food dish made with chopped roti, stir-fried with chicken, vegetables, and aromatic spices, creating a flavorful and satisfying meal.", Ingredients = "Chopped Roti, Chicken, Carrots, Cabbage, Onions, Green Chilies, Eggs, Soy Sauce, Garlic, Ginger, Curry Leaves, Spices", SpecialInformation = "Contains Gluten, Contains Eggs, Contains Soy", Price = 16.50M, ImageUrl="/bleaf/images/modal/chicken_koththu.jpg", ImageThumbnailUrl="/bleaf/images/menu-small/grid/chicken_koththu_mini.png", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "Beef Kottu", Description = "A hearty Sri Lankan street food dish made with chopped roti, stir-fried with tender beef, vegetables, and aromatic spices, creating a rich and flavorful meal.", Ingredients = "Chopped Roti, Beef, Carrots, Cabbage, Onions, Green Chilies, Eggs, Soy Sauce, Garlic, Ginger, Curry Leaves, Spices", SpecialInformation = "Contains Gluten, Contains Eggs, Contains Soy", Price = 16.50M, ImageUrl = "/bleaf/images/modal/beef_kottu.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/beef_kottu_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "Egg Fried Rice", Description = "A classic fried rice dish featuring fluffy rice stir-fried with eggs, vegetables, and a blend of savory sauces, perfect for a quick and satisfying meal.", Ingredients = "Rice, Eggs, Carrots, Peas, Green Onions, Soy Sauce, Garlic, Ginger, Salt, Pepper, Cooking Oil", SpecialInformation = "Vegetarian, Contains Eggs, Contains Soy, Gluten-Free (if soy sauce is gluten-free)", Price = 18.50M, ImageUrl = "/bleaf/images/modal/egg_fried_rice.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_fried_rice_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "Chicken Fried Rice", Description = "A delicious fried rice dish featuring tender chicken pieces stir-fried with fluffy rice, vegetables, and a blend of savory sauces, perfect for a hearty meal.", Ingredients = "Rice, Chicken, Eggs, Carrots, Peas, Green Onions, Soy Sauce, Garlic, Ginger, Salt, Pepper, Cooking Oil", SpecialInformation = "Contains Eggs, Contains Soy, Gluten-Free (if soy sauce is gluten-free)", Price = 10.99M, ImageUrl = "/bleaf/images/modal/chicken_fried_rice.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/chicken_fried_rice_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "Egg Roti and Parata", Description = "A delightful combination of savory egg roti and flaky parathas, perfect for enjoying with your favorite curries or as a standalone meal. This package includes both egg-filled rotis and plain parathas.", Ingredients = "Flour, Eggs, Onions, Green Chilies, Butter or Ghee, Salt, Pepper, Water, Cooking Oil", SpecialInformation = "Contains Gluten, Contains Eggs, Contains Dairy (if made with butter or ghee)", Price = 16.50M, ImageUrl = "/bleaf/images/modal/egg_roti_paratha_mix.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_roti_paratha_mix_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						new Item { Name = "String Hoppers", Description = "Delicate and lacy steamed rice flour noodles, perfect for pairing with curries or sambols.", Ingredients = "Rice Flour, Water, Salt", SpecialInformation = "Gluten-Free", Price = 3.49M, ImageUrl = "/bleaf/images/modal/string_hoppers.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/string_hoppers_thumb.jpg", InStock = true, CategoryId = sriLankanDishesCategory, IsSpecial = false },
						//Beverages
						new Item { Name = "Flat White", Description = "A smooth and velvety coffee made with a shot of espresso and steamed milk, offering a perfect balance of rich coffee flavor and creamy texture.", Ingredients = "Espresso, Steamed Milk", SpecialInformation = "Contains Dairy", Price = 4.80M, ImageUrl = "/bleaf/images/modal/flat_white.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/flat_white_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Cappuccino", Description = "A classic coffee drink featuring a shot of espresso topped with equal parts steamed milk and milk foam, creating a rich and frothy experience.", Ingredients = "Espresso, Steamed Milk, Milk Foam", SpecialInformation = "Contains Dairy", Price = 4.80M, ImageUrl = "/bleaf/images/modal/cappuccino.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/cappuccino_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Mocha", Description = "A delightful blend of espresso, steamed milk, and rich chocolate syrup, topped with whipped cream for a decadent coffee treat.", Ingredients = "Espresso, Steamed Milk, Chocolate Syrup, Whipped Cream", SpecialInformation = "Contains Dairy", Price = 4.80M, ImageUrl = "/bleaf/images/modal/mocha.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/mocha_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Americano", Description = "A simple and bold coffee made by diluting a shot of espresso with hot water, offering a smooth and rich coffee flavor.", Ingredients = "Espresso, Hot Water", SpecialInformation = "None", Price = 4.80M, ImageUrl = "/bleaf/images/modal/americano.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/americano_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Long Black", Description = "A strong and aromatic coffee made by pouring a double shot of espresso over hot water, preserving the crema and offering a robust flavor.", Ingredients = "Double Espresso, Hot Water", SpecialInformation = "None", Price = 4.50M, ImageUrl = "/bleaf/images/modal/long_black.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/long_black_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Hot Chocolate", Description = "A rich and creamy hot chocolate made with steamed milk and high-quality cocoa, topped with whipped cream for a comforting treat.", Ingredients = "Steamed Milk, Cocoa, Whipped Cream", SpecialInformation = "Contains Dairy", Price = 4.80M, ImageUrl = "/bleaf/images/modal/hot_chocolate.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/hot_chocolate_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Tea", Description = "A soothing cup of tea brewed to perfection, available in a variety of flavors including black, green, and herbal teas.", Ingredients = "Tea Leaves, Hot Water", SpecialInformation = "None", Price = 5.00M, ImageUrl = "/bleaf/images/modal/tea.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/tea_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Chai", Description = "A fragrant and spiced chai tea made with a blend of black tea, spices, and steamed milk, offering a warm and comforting experience.", Ingredients = "Black Tea, Spices (Cinnamon, Cardamom, Ginger, etc.), Steamed Milk, Honey (optional)", SpecialInformation = "Contains Dairy", Price = 4.80M, ImageUrl = "/bleaf/images/modal/chai.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/chai_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Milkshake", Description = "A rich and creamy milkshake made with your choice of flavors, blended to perfection with ice cream and milk, and topped with whipped cream.", Ingredients = "Ice Cream, Milk, Whipped Cream, Flavor Syrup (Chocolate, Vanilla, Strawberry, etc.)", SpecialInformation = "Contains Dairy", Price = 7.00M, ImageUrl = "/bleaf/images/modal/milkshake.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/milkshake_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Smoothie", Description = "A refreshing and healthy smoothie made with a blend of fresh fruits, yogurt, and a splash of juice, perfect for a nutritious treat.", Ingredients = "Fresh Fruits (Banana, Berries, Mango, etc.), Yogurt, Juice (Orange, Apple, etc.)", SpecialInformation = "Contains Dairy", Price = 8.00M, ImageUrl = "/bleaf/images/modal/smoothie.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/smoothie_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						new Item { Name = "Iced Coffee", Description = "A chilled and refreshing iced coffee made with a shot of espresso, cold milk, and ice, perfect for a cool pick-me-up.", Ingredients = "Espresso, Cold Milk, Ice, Sweetener (optional)", SpecialInformation = "Contains Dairy", Price = 8.00M, ImageUrl = "/bleaf/images/modal/iced_coffee.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/iced_coffee_thumb.jpg", InStock = true, CategoryId = beveragesCategory, IsSpecial = false },
						//Snacks
						new Item { Name = "Kibula Banis", Description = "A traditional Sri Lankan sweet bun shaped like a tortoise, made with a soft and fluffy dough and a hint of sweetness.", Ingredients = "Flour, Sugar, Yeast, Butter, Milk, Eggs", SpecialInformation = "Contains Gluten, Contains Dairy, Contains Eggs", Price = 3.50M, ImageUrl = "/bleaf/images/modal/kibula_banis.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/kibula_banis_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Egg Rolls", Description = "Crispy and savory egg rolls filled with a delicious mixture of eggs, vegetables, and spices, perfect for a quick snack.", Ingredients = "Eggs, Flour, Vegetables, Spices, Cooking Oil", SpecialInformation = "Contains Gluten, Contains Eggs", Price = 4.00M, ImageUrl = "/bleaf/images/modal/egg_rolls.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/egg_rolls_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Caramelized Onion Buns", Description = "Soft and fluffy buns filled with sweet and savory caramelized onions, perfect for a tasty snack or side dish.", Ingredients = "Flour, Caramelized Onions, Yeast, Butter, Milk, Sugar, Salt", SpecialInformation = "Contains Gluten, Contains Dairy", Price = 4.00M, ImageUrl = "/bleaf/images/modal/caramelized_onion_buns.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/caramelized_onion_buns_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Coconut Roti", Description = "A traditional Sri Lankan flatbread made with freshly grated coconut, flour, and a hint of salt, perfect for a quick snack or side dish.", Ingredients = "Flour, Grated Coconut, Salt, Water", SpecialInformation = "Contains Gluten", Price = 3.00M, ImageUrl = "/bleaf/images/modal/coconut_roti.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/coconut_roti_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Hoppers", Description = "A traditional Sri Lankan bowl-shaped pancake made from a fermented rice flour and coconut milk batter, crispy on the edges and soft in the center.", Ingredients = "Rice Flour, Coconut Milk, Yeast, Sugar, Salt", SpecialInformation = "Gluten-Free", Price = 3.00M, ImageUrl = "/bleaf/images/modal/hoppers.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/hoppers_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Fish Rolls", Description = "Crispy and savory rolls filled with a spiced fish mixture, perfect for a quick and satisfying snack.", Ingredients = "Fish, Flour, Spices, Onions, Green Chilies, Cooking Oil", SpecialInformation = "Contains Gluten, Contains Fish", Price = 4.00M, ImageUrl = "/bleaf/images/modal/fish_rolls.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/fish_rolls_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Patis", Description = "A savory pastry filled with a spiced mixture of meat or vegetables, perfect for a tasty snack or appetizer.", Ingredients = "Flour, Meat or Vegetables, Spices, Onions, Green Chilies, Cooking Oil", SpecialInformation = "Contains Gluten, Contains Meat (if non-vegetarian)", Price = 3.00M, ImageUrl = "/bleaf/images/modal/patis.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/patis_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Fish Bun", Description = "A soft and fluffy bun filled with a spiced fish mixture, perfect for a quick and satisfying snack.", Ingredients = "Flour, Fish, Spices, Onions, Green Chilies, Yeast, Butter, Milk, Sugar, Salt", SpecialInformation = "Contains Gluten, Contains Fish, Contains Dairy", Price = 3.00M, ImageUrl = "/bleaf/images/modal/fish_bun.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/fish_bun_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Vegetable Roti", Description = "A savory flatbread filled with a spiced vegetable mixture, perfect for a quick and satisfying snack.", Ingredients = "Flour, Vegetables, Spices, Onions, Green Chilies, Cooking Oil", SpecialInformation = "Contains Gluten", Price = 4.00M, ImageUrl = "/bleaf/images/modal/vegetable_roti.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/vegetable_roti_thumb.jpg", InStock = true, CategoryId = snacksCategory, IsSpecial = false },
						new Item { Name = "Fish Cutlets", Description="Spicy fish cutlets", Ingredients="Fish, Potatoes, Spices", SpecialInformation="Contains Fish", Price=3.99M, ImageUrl="/bleaf/images/modal/fish-cutlet.png", ImageThumbnailUrl="/bleaf/images/menu-small/grid/fish-cutlet_mini.png", InStock=true, CategoryId=snacksCategory, IsSpecial=false},
						//Desserts
						new Item { Name = "Watalappan", Description="Traditional Sri Lankan dessert", Ingredients="Coconut Milk, Jaggery, Eggs", SpecialInformation="Contains Eggs", Price=4.99M, ImageUrl="/bleaf/images/modal/Watalappan.png", ImageThumbnailUrl="/bleaf/images/menu-small/grid/Watalappan_mini.png", InStock=true, CategoryId=dessertCategory, IsSpecial=false},
						new Item { Name = "Butter Cake", Description = "A classic and rich butter cake with a moist and tender crumb, perfect for any occasion. Enjoy it on its own or with a dollop of whipped cream.", Ingredients = "Flour, Butter, Sugar, Eggs, Baking Powder, Vanilla Extract, Milk", SpecialInformation = "Contains Gluten, Contains Dairy, Contains Eggs", Price = 5.00M, ImageUrl = "/bleaf/images/modal/butter_cake.jpg", ImageThumbnailUrl = "/bleaf/images/menu-small/grid/butter_cake_thumb.jpg", InStock = true, CategoryId = dessertCategory, IsSpecial = false }
					};

					foreach (var i in items)
					{
						context.Items.Add(i);
					}
					context.SaveChanges();
				}

				// Seed Reviews
				if (!context.Reviews.Any())
				{
					var reviews = new Review[]
					{
						new Review{Rating=5, ReviewText="Delicious and spicy!", ReviewerName="Nimal Perera", ReviewerJob="Food Blogger", ItemId=1},
						new Review{Rating=4, ReviewText="Authentic taste!", ReviewerName="Kamal Silva", ReviewerJob="Chef", ItemId=2},
						new Review{Rating=5, ReviewText="Best dessert ever!", ReviewerName="Samanthi Jayasinghe", ReviewerJob="Baker", ItemId=3},
						new Review{Rating=4, ReviewText="Very refreshing!", ReviewerName="Ruwan Fernando", ReviewerJob="Bartender", ItemId=4}
					};

					foreach (var r in reviews)
					{
						context.Reviews.Add(r);
					}
					context.SaveChanges();
				}

				// Seed Orders
				if (!context.Orders.Any())
				{
					var orders = new Order[]
					{
						new Order{UserId=1, AddressId=1, OrderTotal=15.97M, OrderStatus="Completed", PaymentMethod="Credit Card", PaymentStatus="Paid", OrderPlacedAt=DateTime.Now, EstimatedDelivery=DateTime.Now.AddHours(1), DeliveredAt=DateTime.Now.AddHours(1)},
						new Order{UserId=2, AddressId=2, OrderTotal=8.99M, OrderStatus="Pending", PaymentMethod="PayPal", PaymentStatus="Unpaid", OrderPlacedAt=DateTime.Now, EstimatedDelivery=DateTime.Now.AddHours(2)}
					};

					foreach (var o in orders)
					{
						context.Orders.Add(o);
					}
					context.SaveChanges();
				}

				// Seed OrderDetails
				if (!context.OrderDetails.Any())
				{
					var orderDetails = new OrderDetail[]
					{
						new OrderDetail{OrderId=1, ItemId=1, Quantity=2, UnitPrice=3.99M},
						new OrderDetail{OrderId=1, ItemId=3, Quantity=1, UnitPrice=4.99M},
						new OrderDetail{OrderId=2, ItemId=2, Quantity=1, UnitPrice=8.99M}
					};

					foreach (var od in orderDetails)
					{
						context.OrderDetails.Add(od);
					}
					context.SaveChanges();
				}

				// Seed ShoppingCartItems
				if (!context.ShoppingCartItems.Any())
				{
					var shoppingCartItems = new ShoppingCartItem[]
					{
						new ShoppingCartItem{UserId=1, ItemId=4, Quantity=1, SpecialInstructions="Chilled", AddedAt=DateTime.Now},
						new ShoppingCartItem{UserId=2, ItemId=1, Quantity=3, SpecialInstructions="Extra spicy", AddedAt=DateTime.Now}
					};

					foreach (var sci in shoppingCartItems)
					{
						context.ShoppingCartItems.Add(sci);
					}
					context.SaveChanges();
				}

				// Seed Discounts
				if (!context.Discounts.Any())
				{
					var discounts = new Discount[]
					{
						new Discount{Code="SAVE10", Description="Save 10% on your order", DiscountAmount=null, DiscountPercentage=10, ValidFrom=DateTime.Now, ValidTo=DateTime.Now.AddMonths(1), IsActive=true},
						new Discount{Code="FREESHIP", Description="Free shipping on orders over $50", DiscountAmount=0M, DiscountPercentage=null, ValidFrom=DateTime.Now, ValidTo=DateTime.Now.AddMonths(1), IsActive=true}
					};

					foreach (var d in discounts)
					{
						context.Discounts.Add(d);
					}
					context.SaveChanges();
				}

				// Seed Reservations
				if (!context.Reservations.Any())
				{
					var reservations = new Reservation[]
					{
						new Reservation{UserId=1, ReservationDate=DateTime.Now.AddDays(1), NumberOfPeople=2, SpecialRequests="Window seat", ReservationStatus="Pending"},
						new Reservation{UserId=2, ReservationDate=DateTime.Now.AddDays(2), NumberOfPeople=4, SpecialRequests="High chair for baby", ReservationStatus="Confirmed"}
					};

					foreach (var r in reservations)
					{
						context.Reservations.Add(r);
					}
					context.SaveChanges();
				}
			}
		}
	}
}