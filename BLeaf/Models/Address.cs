using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BLeaf.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        public string AddressLine2 { get; set; }

        [StringLength(10, MinimumLength = 4)]
        public string ZipCode { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string Country { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public bool IsPrimary { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Add this navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
