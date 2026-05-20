using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Reservation? Reservation { get; set; }
        public User? Customer { get; set; }
    }
}