using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BLeaf.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000, MinimumLength = 5)]
        public string ReviewText { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ReviewerName { get; set; }

        [StringLength(100)]
        public string ReviewerJob { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
