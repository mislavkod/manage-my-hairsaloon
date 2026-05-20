using System.ComponentModel.DataAnnotations;
using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.ViewModels
{
    public class EditStaffViewModel
    {
        public int Id { get; set; }

        // --- Editable fields ---
        [Required(ErrorMessage = "User is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(0, 10000, ErrorMessage = "Hourly rate must be between $0 and $10,000.")]
        public decimal HourlyRate { get; set; }

        public bool IsAvailable { get; set; }

        // --- Original snapshot (hidden fields) — used for optimistic concurrency check ---
        public int OriginalUserId { get; set; }
        public string? OriginalSpecialization { get; set; }
        public decimal OriginalHourlyRate { get; set; }
        public bool OriginalIsAvailable { get; set; }

        // --- Display data (not submitted / not validated) ---
        public string? StaffFullName { get; set; }
        public List<User>? Users { get; set; }
    }
}
