using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User is required.")]
        public int UserId { get; set; }

        public int HairSalonId { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(0, 10000, ErrorMessage = "Hourly rate must be between $0 and $10,000.")]
        public decimal HourlyRate { get; set; }

        public bool IsAvailable { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User? User { get; set; }
        public HairSalon? HairSalon { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }
}