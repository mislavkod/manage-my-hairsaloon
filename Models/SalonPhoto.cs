namespace manage_my_hairsaloon.Models
{
    public class SalonPhoto
    {
        public int Id { get; set; }

        public int HairSalonId { get; set; }
        public HairSalon? HairSalon { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
