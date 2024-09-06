using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLeaf.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Ingredients { get; set; }

        [StringLength(500)]
        public string SpecialInformation { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        [StringLength(255)]
        public string ImageThumbnailUrl { get; set; }

        public bool? InStock { get; set; } = true;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = default!;

        public bool IsSpecial { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        //public ICollection<Review> Reviews { get; set; }
    }
}