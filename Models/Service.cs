using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int HairSalonId { get; set; }

        [Required(ErrorMessage = "Service name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes.")]
        public int DurationMinutes { get; set; }

        public ServiceCategory Category { get; set; }
        public DateTime? DeletedAt { get; set; }

        public HairSalon? HairSalon { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }
}