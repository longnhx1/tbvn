using System.ComponentModel.DataAnnotations;
using Lab_03.Infrastructure;
using Lab_03.Models;

namespace Lab_03.Models.ViewModels
{
    public class BookingCreateViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public DateTime DefaultTravelDate { get; set; }

        [Required, StringLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string ContactEmail { get; set; } = string.Empty;

        [Range(1, 50)]
        public int Adults { get; set; } = 1;

        [Range(0, 50)]
        public int Children { get; set; } = 0;

        [Required]
        public DateTime TravelDate { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        // Thanh toán
        public BookingPaymentMode PaymentMode { get; set; } = BookingPaymentMode.Full;

        // Số tháng trả góp (chỉ áp dụng khi đủ điều kiện và chọn trả góp).
        [Range(1, 60)]
        public int InstallmentMonths { get; set; } = 3;
    }
}
