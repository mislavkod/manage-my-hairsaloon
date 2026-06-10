using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.DTOs
{
    public class ReviewRequest
    {
        [Required]
        public int ReservationId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
