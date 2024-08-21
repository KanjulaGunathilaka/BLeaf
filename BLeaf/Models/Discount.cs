using System.ComponentModel.DataAnnotations;

namespace BLeaf.Models
{
    public class Discount
    {
        public int DiscountId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public int? DiscountPercentage { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
