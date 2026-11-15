# ShopMini - Hệ thống E-commerce Kiến trúc Microservices

Dự án môn học **Lập trình hướng dịch vụ (SOA / Microservices)** xây dựng hệ thống thương mại điện tử mini dựa trên nền tảng **.NET 9.0 (C# 13)**, **Entity Framework Core 9.0**, **SQL Server** và mô hình **Database-per-Service**.

---

## 📌 Tài liệu chi tiết dự án (Dành cho AI & Developers)

Vui lòng đọc file chi tiết: **[PROJECT_OVERVIEW.md](./PROJECT_OVERVIEW.md)** để nắm toàn bộ:
- Cấu trúc thư mục và chi tiết từng file trong hệ thống.
- Chi tiết 5 Microservices: `UserService`, `ProductService`, `CartService`, `OrderService`, `PaymentService`.
- Sơ đồ cổng mạng (Ports), chuỗi kết nối (Connection Strings), cơ sở dữ liệu.
- Danh mục toàn bộ API Endpoints & Request/Response DTOs.
- Lịch sử các việc đã làm và lộ trình các việc tiếp theo.

---

## 🚀 Các dịch vụ & Cổng mạng (Ports)

| Dịch vụ | Cổng HTTP | Cổng HTTPS | Database |
| :--- | :--- | :--- | :--- |
| **UserService** | `http://localhost:5104` | `https://localhost:7210` | `UserDB` |
| **ProductService** | `http://localhost:5071` | `https://localhost:7117` | `ProductDB` |
| **CartService** | `http://localhost:5290` | `https://localhost:7195` | `CartDB` |
| **OrderService** | `http://localhost:5218` | `https://localhost:7130` | `OrderDB` |
| **PaymentService** | `http://localhost:5151` | `https://localhost:7013` | `PaymentDB` |
| **Gateway.Api** | `http://localhost:5258` | `https://localhost:7028` | *(Đang phát triển)* |

---

## 🛠 Hướng dẫn khởi chạy nhanh

1. **Khởi tạo cơ sở dữ liệu:**
   Chạy các file script trong thư mục `database/` theo thứ tự bằng SQL Server Management Studio (SSMS) hoặc Azure Data Studio:
   - `database/init.sql` (Tạo 5 database, các bảng và ràng buộc)
   - `database/seed.sql` (Chèn dữ liệu mẫu)
   - `database/test-constraints.sql` (Kiểm thử các ràng buộc toàn vẹn)

2. **Biên dịch toàn bộ hệ thống:**
   ```bash
   dotnet build ShopMini.sln
   ```

3. **Chạy một dịch vụ bất kỳ:**
   ```bash
   cd services/ProductService
   dotnet run
   ```