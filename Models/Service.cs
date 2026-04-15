namespace manage_my_hairsaloon.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int HairSalonId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public ServiceCategory Category { get; set; }

        public HairSalon? HairSalon { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }
}