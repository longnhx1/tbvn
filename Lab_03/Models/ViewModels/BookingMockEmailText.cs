using System.Text;
using Lab_03.Models;

namespace Lab_03.Models.ViewModels;

public static class BookingMockEmailText
{
    public static string Build(Booking b)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Kính gửi {b.ContactName},");
        sb.AppendLine();
        sb.AppendLine($"Cảm ơn bạn đã đặt tour tại Travel Booking Việt Nam (bản demo).");
        sb.AppendLine($"Mã đặt chỗ: #{b.Id}");
        sb.AppendLine($"Tour: {b.Tour?.Name ?? "-"}");
        sb.AppendLine($"Điểm đến: {b.Tour?.Destination?.Name ?? "-"}");
        sb.AppendLine($"Khởi hành dự kiến: {b.TravelDate:dd/MM/yyyy}");
        sb.AppendLine($"Điểm xuất phát tour: {b.Tour?.DepartureLocation ?? "-"}");
        sb.AppendLine($"Số khách: {b.Adults} người lớn, {b.Children} trẻ em");
        sb.AppendLine($"Tổng tiền: {b.TotalPrice:N0} đ");
        if (b.IsDemoPaid)
        {
            sb.AppendLine($"Thanh toán demo: Đã hoàn tất");
            sb.AppendLine($"Mã giao dịch (demo): {b.DemoPaymentRef ?? "-"}");
        }
        else
            sb.AppendLine("Thanh toán demo: Chưa hoàn tất — vui lòng vào trang thanh toán mô phỏng.");

        sb.AppendLine();
        sb.AppendLine("Trân trọng,");
        sb.AppendLine("Travel Booking Việt Nam");
        return sb.ToString();
    }
}
