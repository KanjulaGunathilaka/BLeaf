using BLeaf.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLeaf.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Category>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Item
            modelBuilder.Entity<Item>()
                .Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Item>()
                .Property(i => i.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Item>()
                .Property(i => i.InStock)
                .HasDefaultValue(true);

            modelBuilder.Entity<Item>()
                .Property(i => i.IsSpecial)
                .HasDefaultValue(false);

            modelBuilder.Entity<Item>()
                .Property(i => i.Price)
                .HasColumnType("decimal(10, 2)");

            // Review
            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // User
            modelBuilder.Entity<User>()
                .Property(u => u.JoinedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("Customer");

            // Address
            modelBuilder.Entity<Address>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Address>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Address>()
                .Property(a => a.IsPrimary)
                .HasDefaultValue(false);

            modelBuilder.Entity<Address>()
                .HasMany(a => a.Orders)
                .WithOne(o => o.Address)
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderPlacedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentMethod)
                .HasDefaultValue("Credit Card");

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasDefaultValue("Unpaid");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderTotal)
                .HasColumnType("decimal(10, 2)");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal(10, 2)");

            // ShoppingCartItem
            modelBuilder.Entity<ShoppingCartItem>()
                .Property(sci => sci.AddedAt)
                .HasDefaultValueSql("GETDATE()");

            // Discount
            modelBuilder.Entity<Discount>()
                .Property(d => d.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Discount>()
                .Property(d => d.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Discount>()
                .Property(d => d.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Discount>()
                .Property(d => d.DiscountAmount)
                .HasColumnType("decimal(10, 2)");

            // Reservation
            modelBuilder.Entity<Reservation>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Reservation>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationStatus)
                .HasDefaultValue("Pending");
        }
    }
}
