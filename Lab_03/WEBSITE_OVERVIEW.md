# Website Overview (Lab_03 - Web Booking)

## 1) Website dùng để làm gì
Website là một ứng dụng ASP.NET Core MVC cho phép người dùng **xem danh sách tour**, **xem chi tiết tour** và **đặt tour**.  
Luồng thanh toán là mô phỏng theo kiểu **thanh toán 100%** hoặc **trả góp (cọc 30% + trả dần theo kỳ)**, sau đó hiển thị **vé điện tử (demo)** dưới dạng QR/tóm tắt.

## 2) Chức năng chính (Người dùng)

### Trang chủ
- Xem các tour nổi bật và điểm đến phổ biến.
- Tìm nhanh tour/điểm đến.
- Điều hướng tới danh sách tour và trang “Tour đã đặt của tôi”.
- Tham chiếu: `Controllers/HomeController.cs`, `Views/Home/Index.cshtml`.

### Danh sách tour + lọc
Người dùng có thể tìm theo:
- Từ khóa (`q`)
- Nhiều điểm đến (`destinationIds`)
- Giá min/max (`minPrice`, `maxPrice`)
- Khoảng ngày khởi hành (`departFrom`, `departTo`)
- Nơi xuất phát (`departLoc`)
- Tham chiếu: `Controllers/ToursController.cs`, `Views/Tours/Index.cshtml`.

### Chi tiết tour
Hiển thị:
- Thông tin tour (ngày đi, thời lượng, nơi xuất phát, chỗ còn lại)
- Bộ ảnh gallery
- Giá và chính sách hủy/hoàn
- Tham chiếu: `Controllers/ToursController.cs`, `Views/Tours/Details.cshtml`.

### Đặt tour
Quy trình đặt tour gồm:
- Nhập thông tin liên hệ
- Chọn ngày đi
- Nhập số lượng `Người lớn` và `Trẻ em` (trẻ em tính 70% giá)
- Chọn phương thức thanh toán:
  - `Thanh toán 100%`
  - `Trả góp (cọc 30% + trả dần)`:
    - Chỉ hiển thị phần chọn `Số tháng trả góp` khi chọn trả góp
    - Phần hiển thị “tiền phải trả” trên UI là **tiền cọc 30%** (không hiển thị tổng tiền tour)
- Tham chiếu: `Controllers/BookingsController.cs`, `Views/Bookings/Create.cshtml`.

### Xác nhận đặt tour và thanh toán demo
Sau khi tạo booking:
- Nếu chưa “thanh toán demo” thì hiển thị bước tiếp theo (kỳ phải trả hiện tại) + QR mô phỏng
- Người dùng có thể:
  - Thanh toán demo bằng nút giả lập (`MockPayment`)
  - Hoặc thanh toán theo token/QR (`PayDemo`)
- Tham chiếu: `Views/Bookings/Confirmation.cshtml`, `Views/Bookings/PayDemo.cshtml`, `Views/Bookings/MockPayment.cshtml`.

### Xem vé điện tử (demo)
- Nếu đã hoàn tất thanh toán demo thì hiển thị vé (tóm tắt trên QR).
- Tham chiếu: `Views/Bookings/_BookingTicketCard.cshtml`, `Views/Bookings/Ticket.cshtml` (gọi qua action `Ticket`).

### Quản lý tour đã đặt
- Xem lịch sử các booking của người dùng (`MyOrders`)
- Xem chi tiết từng booking (`Details`)
- Hủy booking (`Cancel`) và cập nhật refund theo cancellation policy (trong luồng demo)
- Tham chiếu: `Views/Bookings/MyOrders.cshtml`, `Views/Bookings/Details.cshtml`.

## 3) Luồng “Trả góp” được tính như thế nào (Demo)
Tỷ lệ cọc và lãi theo tháng được cấu hình qua `BookingPaymentOptions`:
- `DownPaymentPercent`: tỷ lệ cọc (mặc định 0.3 = 30%)
- `MonthlyInterestPercent`: lãi suất theo tháng (percent)
- `AllowedInstallmentMonths`: các lựa chọn số tháng trả góp hợp lệ
- Tham chiếu:
  - `Infrastructure/BookingPaymentOptions.cs`
  - `Infrastructure/BookingPaymentCalculator.cs`
  - cấu hình `Lab_03/appsettings.json`

Các phần “kỳ phải trả” (due amount) được tính bởi `BookingPaymentCalculator.GetCurrentDueAmount(booking)`:
- Chưa trả cọc (`PaidInstallmentCount == -1`) => kỳ tiếp theo là **tiền cọc**
- Sau khi trả cọc => chuyển sang các kỳ trả góp tiếp theo
- Tham chiếu: `Infrastructure/BookingPaymentCalculator.cs`.

## 4) Chức năng Admin
Admin bị chặn bởi role `SD.Role_Admin` (Authorize).
Admin có thể:
- Xem dashboard thống kê và booking gần đây
- Quản lý tour (CRUD, ảnh/gallary)
- Quản lý điểm đến (CRUD)
- Quản lý users và gán role (tạo/sửa/xóa)
- Cập nhật trạng thái booking, tính refund demo khi hủy
- Tham chiếu (controller):
  - `Areas/Admin/Controllers/HomeController.cs`
  - `Areas/Admin/Controllers/TourController.cs`
  - `Areas/Admin/Controllers/DestinationController.cs` (Categories)
  - `Areas/Admin/Controllers/UsersController.cs`
  - `Areas/Admin/Controllers/BookingController.cs`

## 5) Danh sách file “quan trọng” nên tham khảo khi viết báo cáo
- `Program.cs`: cấu hình MVC, Identity, routing, DI
- `Controllers/HomeController.cs`: trang chủ
- `Controllers/ToursController.cs`: danh sách + chi tiết tour
- `Controllers/BookingsController.cs`: tạo booking + luồng thanh toán demo + vé điện tử
- `Infrastructure/BookingPaymentCalculator.cs`: tính cọc/kỳ trả/tiền phải trả
- `Infrastructure/BookingPaymentOptions.cs`: cấu hình trả góp
- `Views/Bookings/Create.cshtml`: UI chọn phương thức + hiển thị tiền phải trả theo mode

