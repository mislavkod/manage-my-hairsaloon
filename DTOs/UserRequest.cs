using System.ComponentModel.DataAnnotations;
using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.DTOs
{
    public class UserRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
