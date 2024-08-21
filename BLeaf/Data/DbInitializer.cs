using BLeaf.Models;

namespace BLeaf.Data
{
    public static class DbInitializer
    {
        public static void Seed(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Seed Categories
                if (!context.Categories.Any())
                {
                    var categories = new Category[]
                    {
                    new Category{Name="Appetizers", Description="Start your meal with a bang!"},
                    new Category{Name="Main Courses", Description="Delicious main courses to satisfy your hunger."},
                    new Category{Name="Desserts", Description="Sweet treats to end your meal."}
                    };

                    foreach (var c in categories)
                    {
                        context.Categories.Add(c);
                    }
                    context.SaveChanges();
                }

                // Seed Items
                if (!context.Items.Any())
                {
                    var items = new Item[]
                    {
                    new Item{Name="Spring Rolls", Description="Crispy rolls with vegetables", Ingredients="Cabbage, Carrot, Onion", SpecialInformation="Vegan", Price=5.99M, ImageUrl="/images/spring_rolls.jpg", ImageThumbnailUrl="/images/spring_rolls_thumb.jpg", InStock=true, CategoryId=1, IsSpecial=false},
                    new Item{Name="Grilled Chicken", Description="Juicy grilled chicken breast", Ingredients="Chicken, Spices", SpecialInformation="Gluten-Free", Price=12.99M, ImageUrl="/images/grilled_chicken.jpg", ImageThumbnailUrl="/images/grilled_chicken_thumb.jpg", InStock=true, CategoryId=2, IsSpecial=true},
                    new Item{Name="Chocolate Cake", Description="Rich chocolate cake", Ingredients="Flour, Sugar, Cocoa", SpecialInformation="Contains Nuts", Price=6.99M, ImageUrl="/images/chocolate_cake.jpg", ImageThumbnailUrl="/images/chocolate_cake_thumb.jpg", InStock=true, CategoryId=3, IsSpecial=false}
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
                    new Review{Rating=5, ReviewText="Amazing taste!", ReviewerName="John Doe", ReviewerJob="Food Critic", ItemId=1},
                    new Review{Rating=4, ReviewText="Very good, but a bit salty.", ReviewerName="Jane Smith", ReviewerJob="Chef", ItemId=2},
                    new Review{Rating=5, ReviewText="Best dessert ever!", ReviewerName="Alice Johnson", ReviewerJob="Baker", ItemId=3}
                    };

                    foreach (var r in reviews)
                    {
                        context.Reviews.Add(r);
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
                    new Address{UserId=1, AddressLine1="123 Main St", AddressLine2="", ZipCode="12345", City="Cityville", State="State", Country="Country", PhoneNumber="1234567890", IsPrimary=true},
                    new Address{UserId=2, AddressLine1="456 Admin St", AddressLine2="", ZipCode="67890", City="Admin City", State="Admin State", Country="Admin Country", PhoneNumber="0987654321", IsPrimary=true}
                    };

                    foreach (var a in addresses)
                    {
                        context.Addresses.Add(a);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
