using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.DTOs
{
    public class HairSalonRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [Phone]
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }
    }
}
