using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class HairSalon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon name is required.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<Staff> Staff { get; set; } = new();
        public List<Service> Services { get; set; } = new();
    }
}