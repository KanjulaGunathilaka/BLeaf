using BLeaf.Models;
using Microsoft.EntityFrameworkCore;

namespace BLeaf.Data
{
    public class ApplicationDbContext : DbContext
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

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

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
