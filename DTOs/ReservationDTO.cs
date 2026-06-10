namespace manage_my_hairsaloon.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public UserDTO? Customer { get; set; }
        public StaffDTO? Staff { get; set; }
        public ServiceDTO? Service { get; set; }
    }
}
