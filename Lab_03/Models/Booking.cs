using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_03.Models
{
    public enum BookingPaymentMode
    {
        Full = 0,
        Installment = 1
    }

    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required, StringLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string ContactEmail { get; set; } = string.Empty;

        [Range(1, 50)]
        public int Adults { get; set; }

        [Range(0, 50)]
        public int Children { get; set; }

        public DateTime TravelDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDemoPaid { get; set; }

        public DateTime? DemoPaidAt { get; set; }

        [StringLength(64)]
        public string? DemoPaymentRef { get; set; }

        [StringLength(96)]
        public string? DemoPayToken { get; set; }

        public DateTime? DemoPayTokenExpiresAt { get; set; }

        // ===== Payment plan (demo) =====
        // `IsDemoPaid` remains the single flag to unlock e-ticket in current UI.
        // For installment plans, it will become true only after cọc + đủ kỳ trả góp.
        public BookingPaymentMode PaymentMode { get; set; } = BookingPaymentMode.Full;

        public int? InstallmentMonths { get; set; }

        // Monthly interest rate (percent), e.g. 1 means 1%/month.
        public decimal MonthlyInterestPercent { get; set; }

        // 30% down payment amount (only meaningful when PaymentMode == Installment).
        public decimal DownPaymentAmount { get; set; }

        // Total money already "paid" in the system (deposit + installments).
        public decimal TotalPaid { get; set; }

        // -1 => deposit not paid yet
        // 0..InstallmentMonths => how many installment months have been paid
        public int PaidInstallmentCount { get; set; } = -1;

        // Cancel/refund
        public DateTime? CancelledAt { get; set; }
        public decimal? RefundAmount { get; set; }

        // Tổng tiền phải thanh toán theo kế hoạch trả góp (đã gồm lãi nếu có).
        // Không lưu DB: tính toán từ dữ liệu đã lưu.
        [NotMapped]
        public decimal TotalPayable
        {
            get
            {
                if (PaymentMode == BookingPaymentMode.Full)
                    return TotalPrice;

                if (InstallmentMonths is null or <= 0)
                    return TotalPrice;

                var n = InstallmentMonths.Value;
                var remainingPrincipal = TotalPrice - DownPaymentAmount;
                var principalPerMonth = remainingPrincipal / n;
                var rate = MonthlyInterestPercent / 100m;

                var total = DownPaymentAmount;
                for (var k = 1; k <= n; k++)
                {
                    var outstandingBeforeInterest = remainingPrincipal - principalPerMonth * (k - 1);
                    var interest = outstandingBeforeInterest * rate;
                    total += principalPerMonth + interest;
                }

                return total;
            }
        }
    }

    public static class BookingStatusDisplay
    {
        public static string ToVietnamese(BookingStatus status) => status switch
        {
            BookingStatus.Pending => "Chờ xác nhận",
            BookingStatus.Confirmed => "Đã xác nhận",
            BookingStatus.Cancelled => "Đã hủy",
            BookingStatus.Completed => "Hoàn thành",
            _ => status.ToString()
        };
    }
}
