namespace manage_my_hairsaloon.DTOs
{
    public class StaffDTO
    {
        public int Id { get; set; }
        public string? Specialization { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsAvailable { get; set; }
        public string? HairSalonName { get; set; }
        public UserDTO? User { get; set; }
    }
}
