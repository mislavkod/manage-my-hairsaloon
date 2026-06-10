using System.ComponentModel.DataAnnotations;
using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.DTOs
{
    public class ServiceRequest
    {
        [Required]
        public int HairSalonId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(5, 480)]
        public int DurationMinutes { get; set; }

        [Required]
        public ServiceCategory Category { get; set; }
    }
}
