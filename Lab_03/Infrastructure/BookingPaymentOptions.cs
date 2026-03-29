namespace Lab_03.Infrastructure;

public class BookingPaymentOptions
{
    // Chỉ áp dụng trả góp cho đơn có >= mức khách này.
    public int MinGuestsForInstallment { get; set; } = 3;

    // Tỉ lệ cọc khi trả góp.
    public decimal DownPaymentPercent { get; set; } = 0.30m;

    // Lãi suất theo tháng (đơn vị: percent, ví dụ 1 nghĩa là 1%/tháng).
    public decimal MonthlyInterestPercent { get; set; } = 1.0m;

    // Các lựa chọn số tháng trả góp hợp lệ.
    public int[] AllowedInstallmentMonths { get; set; } = new[] { 3, 6 };

    // Nếu không parse được % từ CancellationPolicy thì fallback vào đây.
    public decimal RefundPercentFallback { get; set; } = 0.75m;

    // Clamp phạm vi hoàn tiền.
    public decimal RefundPercentMin { get; set; } = 0.70m;
    public decimal RefundPercentMax { get; set; } = 0.80m;
}

