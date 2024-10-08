using BLeaf.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(15, MinimumLength = 7)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    public string? BillingAddress { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.Now;

    [StringLength(50)]
    public string Role { get; set; } = "Customer";

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}