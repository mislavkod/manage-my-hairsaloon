namespace manage_my_hairsaloon.Models
{
    public class HairSalon
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Staff> Staff { get; set; } = new();
        public List<Service> Services { get; set; } = new();
    }
}