using System.ComponentModel.DataAnnotations;
using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.ViewModels
{
    public class NewReviewViewModel
    {
        // --- Display data (populated by controller, not validated) ---
        public Reservation? Reservation { get; set; }
        public List<User>? Customers { get; set; }

        // --- Routing (hidden field) ---
        public int SalonId { get; set; }

        // --- Form input ---
        [Required]
        public int ReservationId { get; set; }

        // TODO: When login is implemented, remove CustomerId from the form and replace with:
        //   model.CustomerId = GetCurrentUserId();
        // Then verify reservation.CustomerId == currentUserId before allowing submission.
        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }
    }
}
