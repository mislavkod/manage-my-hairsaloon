using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200)]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Staff? Staff { get; set; }
        public List<Reservation> CustomerReservations { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}