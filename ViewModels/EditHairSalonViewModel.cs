using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.ViewModels
{
    public class EditHairSalonViewModel
    {
        public int Id { get; set; }

        // Editable fields
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

        // Concurrency snapshot
        public string? OriginalName { get; set; }
        public string? OriginalAddress { get; set; }
        public string? OriginalPhoneNumber { get; set; }
        public string? OriginalEmail { get; set; }
    }
}
