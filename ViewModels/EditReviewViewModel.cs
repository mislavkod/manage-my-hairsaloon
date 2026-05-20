using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.ViewModels
{
    public class EditReviewViewModel
    {
        public int Id { get; set; }

        // Editable fields
        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }

        // Concurrency snapshot
        public int OriginalRating { get; set; }
        public string? OriginalComment { get; set; }

        // Display
        public string? CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
