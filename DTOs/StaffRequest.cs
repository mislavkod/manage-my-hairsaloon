using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.DTOs
{
    public class StaffRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int HairSalonId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [Range(0, 10000)]
        public decimal HourlyRate { get; set; }

        public bool IsAvailable { get; set; }
    }
}
