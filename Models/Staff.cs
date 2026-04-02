namespace manage_my_hairsaloon.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HairSalonId { get; set; }
        public string Specialization { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsAvailable { get; set; }

        public User User { get; set; }
        public HairSalon HairSalon { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }
}