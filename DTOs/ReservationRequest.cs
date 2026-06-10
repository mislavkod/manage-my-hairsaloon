using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.DTOs
{
    public class ReservationRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }

        public string? Notes { get; set; }
    }
}
