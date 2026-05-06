using System.ComponentModel.DataAnnotations;
using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.ViewModels
{
    public class NewReservationViewModel
    {
        // --- Display data (populated by GET action) ---
        public HairSalon Salon { get; set; } = null!;
        public List<Service> Services { get; set; } = new();
        public List<Staff> AvailableStaff { get; set; } = new();
        public List<User> Customers { get; set; } = new();

        /// <summary>
        /// 1-hour slots (hour-of-day, e.g. 9 = 09:00, 14 = 14:00) that still have
        /// at least one available staff member free on the selected date.
        /// Empty until a date is submitted.
        /// </summary>
        public List<int> AvailableHours { get; set; } = new();

        // --- Form input ---

        [Required(ErrorMessage = "Please select a service.")]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Please select a staff member.")]
        [Display(Name = "Staff member")]
        public int StaffId { get; set; }

        /// <summary>
        /// Temporary until authentication is added — user picks who they are from the list.
        /// </summary>
        [Required(ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please select a date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime? SelectedDate { get; set; }

        [Required(ErrorMessage = "Please select a time slot.")]
        [Range(9, 18, ErrorMessage = "Time slot must be between 09:00 and 18:00.")]
        [Display(Name = "Time slot")]
        public int? SelectedHour { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
