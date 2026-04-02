namespace manage_my_hairsaloon.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public ReservationStatus Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Customer { get; set; }
        public Staff Staff { get; set; }
        public Service Service { get; set; }
        public List<Review> Reviews { get; set; } = new();
    }
}