using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_03.Models
{
    // Tour: danh mục nghiệp vụ chính của website Travel Booking
    public class Tour
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range(0.01, 1000000000.00)]

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<TourImage>? Images { get; set; }
        public int DestinationId { get; set; }
        public Destination? Destination { get; set; }

        [Required, StringLength(100)]
        public string DepartureLocation { get; set; } = "TP. Ho Chi Minh";

        [Required]
        public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(7);

        [Range(1, 30)]
        public int DurationDays { get; set; } = 3;

        [Range(1, 200)]
        public int MaxGuests { get; set; } = 20;

        [Range(0, 200)]
        public int AvailableSeats { get; set; } = 20;

        [StringLength(200)]
        public string TransportType { get; set; } = "Xe du lich";

        [StringLength(500)]
        public string IncludedServices { get; set; } = "Xe dua don, khach san, huong dan vien";

        [StringLength(500)]
        public string ExcludedServices { get; set; } = "Chi phi ca nhan";

        [StringLength(500)]
        public string CancellationPolicy { get; set; } = "Hoan 70% truoc 7 ngay khoi hanh";

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
