using System.Text.RegularExpressions;
using Lab_03.Models;
using Microsoft.Extensions.Options;

namespace Lab_03.Infrastructure;

public class BookingPaymentCalculator
{
    private readonly BookingPaymentOptions _options;

    public BookingPaymentCalculator(IOptions<BookingPaymentOptions> options)
    {
        _options = options.Value;
    }

    public decimal GetDownPaymentAmount(decimal totalPrice)
        => totalPrice * _options.DownPaymentPercent;

    public decimal GetMonthlyInterestRate(Booking booking)
        => booking.MonthlyInterestPercent / 100m;

    public decimal GetRemainingPrincipal(Booking booking)
    {
        // Gốc trả góp = 70% tổng giá trị đơn (theo nghiệp vụ hiện tại trong project).
        var down = booking.DownPaymentAmount;
        return booking.TotalPrice - down;
    }

    public int GetInstallmentMonths(Booking booking)
    {
        if (booking.InstallmentMonths is null or <= 0)
            throw new InvalidOperationException("InstallmentMonths is required for installment mode.");

        return booking.InstallmentMonths.Value;
    }

    public bool IsDepositPaid(Booking booking)
        => booking.PaymentMode == BookingPaymentMode.Installment && booking.PaidInstallmentCount >= 0;

    public bool IsFullyPaid(Booking booking)
    {
        return booking.PaymentMode switch
        {
            BookingPaymentMode.Full => booking.TotalPaid >= booking.TotalPrice,
            BookingPaymentMode.Installment => booking.PaidInstallmentCount >= GetInstallmentMonths(booking),
            _ => false
        };
    }

    public decimal GetCurrentDueAmount(Booking booking)
    {
        if (booking.PaymentMode == BookingPaymentMode.Full)
            return booking.TotalPrice;

        // Installment
        if (booking.PaidInstallmentCount == -1)
            return booking.DownPaymentAmount;

        var n = GetInstallmentMonths(booking);
        if (booking.PaidInstallmentCount >= n)
            return 0m;

        var nextInstallmentIndex = booking.PaidInstallmentCount + 1; // 1..n
        var remainingPrincipal = GetRemainingPrincipal(booking);
        var principalPerMonth = remainingPrincipal / n; // gốc chia đều

        // Dư nợ trước khi tính lãi ở kỳ k
        var outstandingBeforeInterest =
            remainingPrincipal - principalPerMonth * (nextInstallmentIndex - 1);

        var interest = outstandingBeforeInterest * GetMonthlyInterestRate(booking);

        return principalPerMonth + interest;
    }

    public string GetCurrentDueLabel(Booking booking)
    {
        if (booking.PaymentMode == BookingPaymentMode.Full)
            return "Thanh toán 100%";

        if (booking.PaidInstallmentCount == -1)
            return "Đặt cọc 30%";

        var n = GetInstallmentMonths(booking);
        var k = booking.PaidInstallmentCount + 1;
        return $"Trả góp tháng {k}/{n}";
    }

    public decimal GetTotalPayable(Booking booking)
    {
        if (booking.PaymentMode == BookingPaymentMode.Full)
            return booking.TotalPrice;

        // deposit
        var total = booking.DownPaymentAmount;

        var n = GetInstallmentMonths(booking);
        var remainingPrincipal = GetRemainingPrincipal(booking);
        var principalPerMonth = remainingPrincipal / n;
        var rate = GetMonthlyInterestRate(booking);

        for (var k = 1; k <= n; k++)
        {
            var outstandingBeforeInterest = remainingPrincipal - principalPerMonth * (k - 1);
            var interest = outstandingBeforeInterest * rate;
            total += principalPerMonth + interest;
        }

        return total;
    }

    public decimal GetRefundPercentFromCancellationPolicy(string? cancellationPolicy)
    {
        if (string.IsNullOrWhiteSpace(cancellationPolicy))
            return _options.RefundPercentFallback;

        // Ví dụ: "Hoan 70% truoc 7 ngay khoi hanh"
        var match = Regex.Match(cancellationPolicy, @"(?i)(\d{1,3})\s*%");
        if (!match.Success || !decimal.TryParse(match.Groups[1].Value, out var percentValue))
            return _options.RefundPercentFallback;

        var p = percentValue / 100m;
        if (p < _options.RefundPercentMin) p = _options.RefundPercentMin;
        if (p > _options.RefundPercentMax) p = _options.RefundPercentMax;
        return p;
    }
}

