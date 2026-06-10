namespace manage_my_hairsaloon.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDTO? Customer { get; set; }
    }
}
