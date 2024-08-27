using BLeaf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Seed Admin User
                var adminUser = await userManager.FindByEmailAsync("admin@example.com");
                if (adminUser == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = "admin@example.com",
                        Email = "admin@example.com",
                        EmailConfirmed = true
                    };

                    var createUser = await userManager.CreateAsync(user, "Admin@123");
                    if (createUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }

                // Seed Categories
                if (!context.Categories.Any())
                {
                    var categories = new Category[]
                    {
                        new Category{Name="Appetizers", Description="Start your meal with a bang!"},
                        new Category{Name="Main Courses", Description="Delicious main courses to satisfy your hunger."},
                        new Category{Name="Desserts", Description="Sweet treats to end your meal."},
                        new Category{Name="Beverages", Description="Refreshing drinks to complement your meal."}
                    };

                    foreach (var c in categories)
                    {
                        context.Categories.Add(c);
                    }
                    context.SaveChanges();
                }

                // Seed Users
                if (!context.Users.Any())
                {
                    var users = new User[]
                    {
                        new User{FullName="John Doe", Email="john@example.com", PhoneNumber="1234567890", BillingAddress="123 Main St", PasswordHash="hashedpassword", JoinedAt=DateTime.Now, Role="Customer"},
                        new User{FullName="Admin User", Email="admin@example.com", PhoneNumber="0987654321", BillingAddress="456 Admin St", PasswordHash="adminhashedpassword", JoinedAt=DateTime.Now, Role="Admin"}
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

                // Seed Items
                if (!context.Items.Any())
                {
                    var items = new Item[]
                    {
                        new Item{Name="Fish Cutlets", Description="Spicy fish cutlets", Ingredients="Fish, Potatoes, Spices", SpecialInformation="Contains Fish", Price=3.99M, ImageUrl="/images/fish_cutlets.jpg", ImageThumbnailUrl="/images/fish_cutlets_thumb.jpg", InStock=true, CategoryId=1, IsSpecial=false},
                        new Item{Name="Chicken Kottu", Description="Sri Lankan street food", Ingredients="Chicken, Roti, Vegetables", SpecialInformation="Spicy", Price=8.99M, ImageUrl="/images/chicken_kottu.jpg", ImageThumbnailUrl="/images/chicken_kottu_thumb.jpg", InStock=true, CategoryId=2, IsSpecial=true},
                        new Item{Name="Watalappan", Description="Traditional Sri Lankan dessert", Ingredients="Coconut Milk, Jaggery, Eggs", SpecialInformation="Contains Eggs", Price=4.99M, ImageUrl="/images/watalappan.jpg", ImageThumbnailUrl="/images/watalappan_thumb.jpg", InStock=true, CategoryId=3, IsSpecial=false},
                        new Item{Name="King Coconut", Description="Refreshing king coconut water", Ingredients="King Coconut", SpecialInformation="Natural Drink", Price=2.99M, ImageUrl="/images/king_coconut.jpg", ImageThumbnailUrl="/images/king_coconut_thumb.jpg", InStock=true, CategoryId=4, IsSpecial=false}
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