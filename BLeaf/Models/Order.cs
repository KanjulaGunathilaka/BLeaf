using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BLeaf.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal OrderTotal { get; set; }

        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending";

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Credit Card";

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid";

        public DateTime OrderPlacedAt { get; set; } = DateTime.Now;
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
