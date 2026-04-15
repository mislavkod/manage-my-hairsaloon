namespace manage_my_hairsaloon.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Reservation? Reservation { get; set; }
        public User? Customer { get; set; }
    }
}