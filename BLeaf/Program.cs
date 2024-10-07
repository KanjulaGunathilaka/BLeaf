using BLeaf.Data;
using BLeaf.Models.IRepository;
using BLeaf.Models.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContextConnection")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BLeaf}/{action=Index}/{id?}");

DbInitializer.Seed(app).Wait();
await CreateRolesAndAdminUser(app.Services);

app.Run();

async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var normalizedRoleName = roleName.ToUpper();
            var roleExist = await roleManager.Roles.AnyAsync(r => r.NormalizedName == normalizedRoleName);
            if (!roleExist)
            {
                Console.WriteLine($"Creating role: {roleName}");
                roleResult = await roleManager.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = normalizedRoleName });
            }
            else
            {
                Console.WriteLine($"Role already exists: {roleName}");
            }
        }

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
    }
}