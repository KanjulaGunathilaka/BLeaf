using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BLeaf.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public DateTime ReservationDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? NumberOfPeople { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [StringLength(50)]
        public string ReservationStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}