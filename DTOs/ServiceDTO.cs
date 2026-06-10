namespace manage_my_hairsaloon.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? HairSalonName { get; set; }
    }
}
