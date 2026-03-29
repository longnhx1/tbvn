namespace Lab_03.Models
{
    // TourImage: ảnh chi tiết gắn với từng tour
    public class TourImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int TourId { get; set; }
        public Tour? Tour { get; set; }
    }
}
