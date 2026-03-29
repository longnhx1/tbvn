# ✈️ TravelBooking

Ứng dụng web đặt tour du lịch xây dựng bằng **ASP.NET Core MVC**, sử dụng Entity Framework Core (Code First), ASP.NET Core Identity và SQL Server.

> 📖 Hướng dẫn setup dựa theo **Giáo trình Thực hành Lập trình Web - HUTECH (ấn bản 2024)**

---

## 📋 Yêu cầu hệ thống

| Công cụ | Phiên bản |
|---|---|
| Visual Studio | 2022 (Community / Professional) |
| .NET | 8.0 |
| SQL Server Express | 2019 hoặc 2022 |
| Git | Bất kỳ |

---

## 📦 NuGet Packages sử dụng

Các package đã được cài sẵn trong project (không cần cài lại thủ công). Để kiểm tra, vào **Tools → NuGet Package Manager → Manage NuGet Packages for Solution**:

| Package | Phiên bản |
|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.3 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.3 |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.3 |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.3 |
| `Microsoft.AspNetCore.Identity.UI` | 8.0.3 |

---

## 🚀 Hướng dẫn cài đặt

### Bước 1 — Clone repository

```bash
git clone https://github.com/longnhx1/TravelBooking.git
cd TravelBooking
```

### Bước 2 — Mở project trong Visual Studio

1. Mở file **`TravelBooking.sln`** bằng Visual Studio 2022.
2. Chờ Visual Studio tự động restore NuGet packages.
3. Nếu không tự restore, click chuột phải vào **Solution → Restore NuGet Packages**.

### Bước 3 — Kiểm tra Connection String

Mở file **`appsettings.json`**, kiểm tra phần `ConnectionStrings`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Lab03;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> ⚠️ **Quan trọng:** Thay `Server=` bằng đúng tên Server name trên máy bạn.  
> Cách xem Server name: Mở **SQL Server Management Studio (SSMS)** → tên hiển thị ở ô *Server name* khi kết nối chính là giá trị cần điền.

**Ví dụ một số tên phổ biến:**

| Trường hợp | Server name |
|---|---|
| Mặc định khi cài Express | `localhost\SQLEXPRESS` |
| Tên máy tính | `DESKTOP-XXXXX\SQLEXPRESS` |
| LocalDB | `(LocalDB)\MSSQLLocalDB` |

### Bước 4 — Tạo Migration và Database

Mở **Package Manager Console**: **Tools → NuGet Package Manager → Package Manager Console**

Đảm bảo **Default project** đang chọn đúng project chính, sau đó chạy lần lượt:

```powershell
# Tạo migration (nếu chưa có)
Add-Migration Initial

# Tạo database và áp dụng migration
Update-Database
```

Nếu project đã có sẵn migration, chỉ cần chạy:

```powershell
Update-Database
```

Sau khi chạy xong, database **`Lab03`** sẽ được tạo tự động trên SQL Server.  
Mở SSMS để kiểm tra: database `Lab03` xuất hiện là thành công ✅

### Bước 5 — Chạy ứng dụng

Nhấn **F5** hoặc **Ctrl + F5** để build và chạy project.

Ứng dụng sẽ tự mở trên trình duyệt tại địa chỉ dạng: `https://localhost:xxxx`

---

## 🛠️ Công nghệ sử dụng

- **ASP.NET Core MVC 8** — Framework chính
- **Entity Framework Core 8** (Code First) — ORM, quản lý database
- **ASP.NET Core Identity** — Đăng ký, đăng nhập, phân quyền người dùng
- **LINQ** — Truy vấn dữ liệu
- **SQL Server Express** — Database
- **Razor Pages** — Giao diện Identity (Login, Register)
- **Bootstrap** — Giao diện người dùng

---

## 📁 Cấu trúc project

```
TravelBooking/
├── Areas/
│   └── Identity/             # Trang Login, Register (Scaffolded)
├── Controllers/              # Xử lý request, điều hướng
├── Models/
│   ├── ApplicationDbContext.cs   # DbContext kế thừa IdentityDbContext
│   ├── ApplicationUser.cs        # Mở rộng IdentityUser (FullName, Address,...)
│   └── Migrations/               # EF Migration files
├── Repositories/             # Interface và EF Repository
├── Views/                    # Giao diện Razor (.cshtml)
├── wwwroot/                  # CSS, JS, hình ảnh tĩnh
├── appsettings.json          # Cấu hình app và connection string
└── Program.cs                # Cấu hình DI, Identity, Middleware
```

---

## ❓ Xử lý lỗi thường gặp

**Lỗi: `A network-related or instance-specific error`**
- SQL Server chưa chạy → Mở **Services** (Win + R → `services.msc`) → tìm **SQL Server (SQLEXPRESS)** → **Start**.
- Hoặc tên Server name trong `appsettings.json` chưa đúng → Mở SSMS để kiểm tra lại.

**Lỗi: `There is already an object named '...' in the database`**
- Database đang bị lệch migration. Xóa database trong SSMS rồi chạy lại `Update-Database`.

**Lỗi: `The model backing the context has changed`**
- Chạy lại `Update-Database` trong Package Manager Console.

**Lỗi: NuGet packages bị thiếu**
- Click chuột phải vào Solution → **Restore NuGet Packages**.

**Lỗi: Port bị chiếm**
- Mở `Properties/launchSettings.json` và đổi số port sang giá trị khác.

---

## 👥 Thành viên nhóm

| Tên | MSSV | Vai trò |
|---|---|---|
|  |  |  |
|  |  |  |

---

> 📌 Nếu gặp vấn đề trong quá trình setup, liên hệ nhóm trưởng hoặc tạo **Issue** trên GitHub.
