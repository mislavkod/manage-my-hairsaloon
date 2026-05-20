using manage_my_hairsaloon.Models;
using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        // Editable fields
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; }

        // Concurrency snapshot
        public string? OriginalEmail { get; set; }
        public string? OriginalFirstName { get; set; }
        public string? OriginalLastName { get; set; }
        public string? OriginalPhoneNumber { get; set; }
        public UserRole OriginalRole { get; set; }
    }
}
