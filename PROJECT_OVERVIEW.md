# BÁO CÁO TOÀN DIỆN DỰ ÁN SHOPMINI (MINI E-COMMERCE MICROSERVICES)
> **Dành cho các mô hình AI và nhà phát triển:** Tài liệu này cung cấp toàn bộ bức tranh kiến trúc, cấu trúc thư mục, danh mục chi tiết từng file, cơ sở dữ liệu, các nghiệp vụ đã hoàn thành, trạng thái hiện tại và các bước tiếp theo của dự án **ShopMini**.

---

## 1. TỔNG QUAN DỰ ÁN (PROJECT OVERVIEW)

- **Tên dự án:** ShopMini (Mini E-commerce System)
- **Học phần / Ngữ cảnh:** Lập trình hướng dịch vụ (Service-Oriented Architecture - SOA / Microservices)
- **Kiến trúc cốt lõi:** Microservices Architecture kết hợp mô hình **Database-per-Service** (Mỗi dịch vụ sở hữu cơ sở dữ liệu riêng biệt).
- **Mô hình lập trình backend:** Kiến trúc phân tầng 3 lớp chuẩn enterprise: `Controller` -> `Service` (Business Logic) -> `Repository` (Data Access) -> `DbContext` (EF Core) kết hợp `DTOs` (Data Transfer Objects).
- **Tech Stack:**
  - **Platform & Framework:** .NET 9.0 (C# 13), ASP.NET Core Web API.
  - **ORM:** Entity Framework Core 9.0 (`Microsoft.EntityFrameworkCore.SqlServer`).
  - **Security / Hashing:** `BCrypt.Net-Next` (4.2.0) để băm mật khẩu người dùng chuẩn bảo mật cao.
  - **Cơ sở dữ liệu:** Microsoft SQL Server (gồm 5 database độc lập: `UserDB`, `ProductDB`, `CartDB`, `OrderDB`, `PaymentDB`).
  - **API Documentation:** Microsoft.AspNetCore.OpenApi (.NET 9 OpenAPI).
  - **Kiến trúc giải pháp (Solution):** File `ShopMini.sln` quản lý 6 project .NET (5 microservices + 1 gateway).

---

## 2. BẢNG PHÂN BỔ CỔNG MẠNG (PORTS & NETWORK MAPPING)

Tất cả các dịch vụ đã được cấu hình trong `launchSettings.json` để có thể chạy đồng thời độc lập trên máy local:

| Dịch vụ (Service) | Cổng HTTP | Cổng HTTPS | Cơ sở dữ liệu tương ứng | Chuỗi kết nối (DefaultConnection) |
| :--- | :--- | :--- | :--- | :--- |
| **UserService** | `http://localhost:5104` | `https://localhost:7210` | `UserDB` | `Server=localhost\SQLEXPRESS;Database=UserDB;...` |
| **ProductService** | `http://localhost:5071` | `https://localhost:7117` | `ProductDB` | `Server=localhost\SQLEXPRESS;Database=ProductDB;...` |
| **CartService** | `http://localhost:5290` | `https://localhost:7195` | `CartDB` | `Server=localhost\SQLEXPRESS;Database=CartDB;...` |
| **OrderService** | `http://localhost:5218` | `https://localhost:7130` | `OrderDB` | `Server=localhost\SQLEXPRESS;Database=OrderDB;...` |
| **PaymentService** | `http://localhost:5151` | `https://localhost:7013` | `PaymentDB` | `Server=localhost\SQLEXPRESS;Database=PaymentDB;...` |
| **Gateway.Api** | `http://localhost:5258` | `https://localhost:7028` | *(Không dùng DB)* | *(Ocelot API Gateway điều hướng toàn bộ `/api/*`)* |

---

## 3. CÁC CÔNG VIỆC ĐÃ HOÀN THÀNH (ACCOMPLISHED TASKS)

Theo tiến trình phát triển và lịch sử Git commit (`SonDev` / `ThinhDev`), toàn bộ các hạng mục sau đã được thiết kế, lập trình và kiểm thử thành công:

### Giai đoạn 1: Khởi tạo và thiết kế Database Microservices
1. **Thiết kế phân rã Database (Database-per-Service):** Tách bạch 5 cơ sở dữ liệu trên SQL Server, không dùng foreign key vật lý liên cơ sở dữ liệu (`Cross-DB Foreign Keys`) để đảm bảo tính tự chủ tuyệt đối của microservices (Loose Coupling).
2. **Kịch bản khởi tạo (`database/init.sql`):** 
   - Tạo 5 database: `UserDB`, `ProductDB`, `CartDB`, `OrderDB`, `PaymentDB`.
   - Cấu hình khóa chính `UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID()`.
   - Các ràng buộc toàn vẹn: CHECK giá tiền `Price > 0`, số lượng `Stock >= 0`, `Quantity > 0`, CHECK trạng thái enum chuỗi cho `Orders` và `Payments`.
   - Cấu hình Unique Filtered Index: Mỗi user chỉ có tối đa 1 địa chỉ mặc định (`UX_Addresses_OneDefaultPerUser`), mỗi user chỉ có 1 giỏ hàng active (`UX_Carts_OneActiveCartPerUser`), mã giao dịch duy nhất khi không null (`UX_Payments_TransactionCode`), mỗi đơn hàng chỉ có 1 thanh toán (`UX_Payments_OrderId`).
3. **Kịch bản dữ liệu mẫu (`database/seed.sql`):** Khởi tạo dữ liệu mẫu với GUID cố định cho User (Admin, Khách hàng), Địa chỉ, Danh mục, Sản phẩm, Giỏ hàng, Đơn hàng, và Giao dịch thanh toán mẫu.
4. **Bộ kiểm thử ràng buộc tự động (`database/test-constraints.sql`):** Bộ kịch bản SQL kiểm thử tự động toàn diện các ràng buộc (PK, FK nội bộ, Check, Unique, Filtered Index, Default value) với cơ chế `TRY...CATCH...ROLLBACK`.

### Giai đoạn 2: Xây dựng UserService (Quản lý User, Role, Address)
1. **Models & Entity Framework Mapping (`UserDbContext`):**
   - Ánh xạ bảng `Users`, `Roles`, `Addresses` bằng Fluent API.
   - Quan hệ 1-N: `Role -> Users`, `User -> Addresses`.
2. **Bảo mật mật khẩu:** Tích hợp thư viện `BCrypt.Net-Next`, tự động băm mật khẩu khi tạo user (`BCrypt.HashPassword`).
3. **Quản lý tài khoản:** Hỗ trợ tạo mới, kiểm tra trùng email, cập nhật họ tên, và xóa mềm (`Soft Delete` chuyển `IsActive = false`).
4. **Nghiệp vụ địa chỉ thông minh (`AddressService`):**
   - Tự động đặt địa chỉ đầu tiên của user làm địa chỉ mặc định.
   - Khi chọn một địa chỉ làm mặc định, tự động hủy mặc định của các địa chỉ khác của user đó.
   - Khi xóa địa chỉ mặc định, tự động chọn địa chỉ còn lại đầu tiên làm mặc định mới.

### Giai đoạn 3: Xây dựng ProductService (Quản lý Danh mục & Sản phẩm)
1. **Models & Data:** `Categories` và `Products` với đầy đủ ràng buộc độ dài, check constraints, index trên `CategoryId`.
2. **Xử lý vòng lặp JSON:** Cấu hình `ReferenceHandler.IgnoreCycles` trong `Program.cs` để giải quyết triệt để lỗi tuần tự hóa quan hệ 2 chiều giữa Category và Product.
3. **Validation nghiệp vụ:** Kiểm tra bắt buộc tên sản phẩm, giá > 0, tồn kho >= 0, xác thực danh mục có tồn tại và đang hoạt động (`IsActive == true`) trước khi thêm/sửa sản phẩm.
4. **CRUD RESTful API:** Cung cấp đầy đủ API cho cả Category và Product.

### Giai đoạn 4: Xây dựng CartService (Quản lý Giỏ hàng)
1. **Models & Data:** `Carts` và `CartItems` với Unique index cặp `(CartId, ProductId)`.
2. **Nghiệp vụ giỏ hàng tự động:** Hàm `GetOrCreateCartAsync` - nếu user chưa có giỏ hàng, hệ thống tự sinh giỏ hàng mới.
3. **Thao tác giỏ hàng:**
   - Thêm sản phẩm vào giỏ: nếu đã có trong giỏ thì tăng `Quantity`, nếu chưa thì thêm mới `CartItem`.
   - Cập nhật số lượng item cụ thể.
   - Xóa một item khỏi giỏ hàng.
   - Dọn sạch giỏ hàng (`ClearCart`).

### Giai đoạn 5: Xây dựng OrderService (Quản lý Đơn hàng)
1. **Models & Data:** `Orders` và `OrderItems`.
   - Cột tính toán lưu trữ (`Persisted Computed Column`): `Subtotal AS (UnitPrice * Quantity) PERSISTED`.
2. **State Machine (Máy trạng thái đơn hàng):** Quy tắc chuyển đổi trạng thái cực kỳ chặt chẽ trong `OrderService.cs`:
   - `PendingPayment` -> `PaymentFailed` hoặc `PendingConfirmation`
   - `PendingConfirmation` -> `Confirmed` hoặc `Cancelled`
   - `Confirmed` -> `Shipping` hoặc `Cancelled`
   - `Shipping` -> `Completed`
3. **Nghiệp vụ hủy đơn:** Chỉ cho phép khách hàng hủy đơn khi đơn hàng đang ở trạng thái `PendingPayment`, `PendingConfirmation`, hoặc `Confirmed`. Kiểm tra quyền sở hữu (`UserId == order.UserId`).
4. **Serialization:** Cấu hình `JsonStringEnumConverter` để hiển thị trạng thái bằng chuỗi text (ví dụ: `"PendingPayment"`) thay vì số nguyên thô.

### Giai đoạn 6: Xây dựng PaymentService (Quản lý Thanh toán)
1. **Models & Enums:**
   - `PaymentMethod`: `COD` (Thanh toán khi nhận hàng), `Simulation` (Mô phỏng thanh toán trực tuyến).
   - `PaymentStatus`: `Unpaid`, `Pending`, `Paid`, `Failed`, `Refunded`.
2. **State Machine thanh toán:**
   - `Pending` -> `Paid` hoặc `Failed`
   - `Paid` -> `Refunded`
3. **Quy tắc tính duy nhất:** Mỗi đơn hàng (`OrderId`) chỉ có tối đa một bản ghi thanh toán (`UX_Payments_OrderId`). Mã giao dịch (`TransactionCode`) là duy nhất khi được cung cấp.

### Giai đoạn 7: Kiểm thử biên dịch và tính toàn vẹn (Build Verification)
- Toàn bộ solution `ShopMini.sln` gồm 6 projects được build thành công 100% với **0 Error(s)** và **0 Warning(s)** trên .NET 9.0 SDK.

### Giai đoạn 8: Hoàn thiện API Gateway (Ocelot Reverse Proxy)
1. Cài đặt thư viện `Ocelot (25.0.2)` cho `Gateway.Api`.
2. Thiết lập kịch bản định tuyến toàn diện trong `ocelot.json` bao gồm cả root paths và wildcard subpaths `{everything}` cho toàn bộ 5 microservices (`/api/users`, `/api/roles`, `/api/addresses`, `/api/products`, `/api/categories`, `/api/carts`, `/api/orders`, `/api/payments`).
3. Cấu hình chính sách CORS `AllowAll` trong `Program.cs` cho phép Client Frontend gọi API không bị chặn cross-origin.

### Giai đoạn 9: Hoàn thiện toàn diện Frontend kết nối Backend Controllers
1. Xây dựng đầy đủ 100% các trang giao diện chuẩn Bootstrap 5, FontAwesome, Responsive di động:
   - `index.html`: Trang chủ mua sắm, hero banner, hiển thị sản phẩm nổi bật, phân loại danh mục.
   - `pages/products.html`: Danh mục toàn bộ sản phẩm, tìm kiếm từ khóa, sắp xếp giá, lọc danh mục.
   - `pages/product-detail.html`: Xem chi tiết sản phẩm, tồn kho, điều chỉnh số lượng và thêm vào giỏ.
   - `pages/cart.html`: Giỏ hàng tương tác, tăng giảm số lượng, xóa item, form đặt hàng và chọn phương thức thanh toán.
   - `pages/orders.html`: Theo dõi lịch sử đơn hàng, cập nhật trạng thái đơn, hủy đơn, thực hiện thanh toán mô phỏng.
   - `pages/login.html`: Đăng nhập, đăng ký tài khoản mới (Customer), điền nhanh tài khoản test mẫu.
2. Viết toàn bộ mã nguồn JavaScript kết nối API:
   - `js/api.js`: Bộ hàm gọi API tập trung qua Gateway (hoặc direct fallback), định dạng tiền tệ VND, hệ thống Toast alert.
   - `js/auth.js`: Quản lý phiên người dùng trong `localStorage`, đăng nhập, đăng ký, đăng xuất.
   - `js/products.js`: Xử lý dữ liệu danh mục & sản phẩm từ ProductService.
   - `js/cart.js`: Đồng bộ hóa giỏ hàng với CartService, điều hướng đặt hàng qua OrderService và PaymentService.
   - `js/main.js`: Render Navbar & Footer dùng chung, cập nhật huy hiệu số lượng món hàng trên Navbar thời gian thực.
   - `css/style.css` & `css/responsive.css`: Định dạng giao diện hiện đại, chuyên nghiệp, responsive.

---

## 4. CÂY THƯ MỤC CHI TIẾT VÀ BẢN ĐỒ TOÀN BỘ FILE (COMPLETE FILE DIRECTORY MAP)

Dưới đây là sơ đồ chi tiết vị trí và nhiệm vụ của **từng file** trong cây thư mục dự án:

```
ShopMini/
│
├── .gitignore                                      # Khai báo loại trừ git (bin, obj, .vs, logs, v.v.)
├── docker-compose.yml                              # File cấu hình điều phối multi-container Docker (Placeholder)
├── README.md                                       # Giới thiệu sơ lược dự án
├── PROJECT_OVERVIEW.md                             # [FILE NÀY] Tài liệu tổng quan kiến trúc chi tiết dành cho AI & Dev
├── ShopMini.sln                                    # File Solution quản trị toàn bộ 6 project .NET
├── run-all.bat                                     # Kịch bản Batch 1-click khởi chạy đồng thời Gateway và 5 Microservices
│
├── database/                                       # THƯ MỤC CƠ SỞ DỮ LIỆU SQL SERVER
│   ├── init.sql                                    # Kịch bản DDL tạo 5 cơ sở dữ liệu, các bảng, ràng buộc & index
│   ├── seed.sql                                    # Kịch bản DML chèn dữ liệu mẫu ban đầu (idempotent, GUID cố định)
│   └── test-constraints.sql                       # Kịch bản kiểm thử tự động tính toàn vẹn và các ràng buộc dữ liệu
│
├── gateway/                                        # THƯ MỤC API GATEWAY
│   ├── Dockerfile                                  # Kịch bản đóng gói container Docker cho Gateway (Placeholder)
│   └── Gateway.Api/                                # ASP.NET Core Web API Gateway Project
│       ├── Gateway.Api.csproj                      # Định nghĩa project .NET 9 cho Gateway
│       ├── Program.cs                              # Điểm khởi chạy của Gateway API
│       ├── appsettings.json                        # Cấu hình môi trường Production cho Gateway
│       ├── appsettings.Development.json            # Cấu hình môi trường Development cho Gateway
│       ├── Gateway.Api.http                        # File kiểm thử HTTP request của Gateway
│       ├── WeatherForecast.cs                      # Template mặc định của ASP.NET Core
│       ├── Controllers/
│       │   └── WeatherForecastController.cs        # Template controller mặc định
│       └── Properties/
│           └── launchSettings.json                 # Cấu hình port (HTTP 5258, HTTPS 7028)
│
├── frontend/                                       # THƯ MỤC GIAO DIỆN CLIENT (STATIC HTML/CSS/JS)
│   ├── Dockerfile                                  # Đóng gói container Nginx cho Frontend (Placeholder)
│   ├── index.html                                  # Trang chủ mua sắm
│   ├── css/
│   │   ├── style.css                               # CSS định dạng giao diện chính, animation, stepper
│   │   └── responsive.css                          # CSS tối ưu hiển thị trên di động / tablet
│   ├── js/
│   │   ├── admin.js                                # Script quản trị: Dashboard KPI, CRUD sản phẩm, State Machine đơn hàng
│   │   ├── api.js                                  # Cấu hình base URL Gateway 5258, hàm gọi API, demo QR generator, toast
│   │   ├── auth.js                                 # Xử lý đăng nhập, đăng ký, phiên người dùng & phân quyền Admin/Customer
│   │   ├── cart.js                                 # Xử lý đồng bộ giỏ hàng, đặt hàng COD và hiển thị QR thanh toán Demo
│   │   ├── main.js                                 # Script khởi tạo chung, render menu, header, badge giỏ hàng thời gian thực
│   │   └── products.js                             # Render danh sách sản phẩm, bộ lọc danh mục, tìm kiếm, xem chi tiết
│   └── pages/
│       ├── admin.html                              # Bảng điều khiển Quản trị (Doanh thu, Tình trạng đơn, Quản lý SP)
│       ├── cart.html                               # Trang xem và chỉnh sửa giỏ hàng, quét QR Demo / COD
│       ├── login.html                              # Trang đăng nhập và đăng ký tài khoản (tự chuyển Admin / Customer)
│       ├── orders.html                             # Trang xem lịch sử đơn hàng, thanh toán QR, hủy đơn hàng
│       ├── product-detail.html                     # Trang xem chi tiết một sản phẩm, tồn kho, mua ngay
│       └── products.html                           # Trang danh mục duyệt tất cả sản phẩm
│
└── services/                                       # THƯ MỤC CHỨA 5 MICROSERVICES BACKEND
    │
    ├── UserService/                                # DỊCH VỤ NGƯỜI DÙNG & PHÂN QUYỀN (Port: 5104)
    │   ├── UserService.csproj                      # Cấu hình project, gói BCrypt.Net-Next, EF Core SqlServer
    │   ├── Program.cs                              # Đăng ký DI (Repositories, Services), DbContext, OpenAPI
    │   ├── appsettings.json                        # Chuỗi kết nối UserDB
    │   ├── appsettings.Development.json            # Cấu hình môi trường dev
    │   ├── Dockerfile                              # Kịch bản Docker của UserService
    │   ├── UserService.http                        # Request test HTTP
    │   ├── WeatherForecast.cs                      # Template mặc định
    │   ├── Properties/
    │   │   └── launchSettings.json                 # Cấu hình cổng HTTP 5104 / HTTPS 7210
    │   ├── Models/                                 # Entity đại diện bảng cơ sở dữ liệu
    │   │   ├── User.cs                             # Entity User (UserId, FullName, Email, PasswordHash, RoleId, v.v.)
    │   │   ├── Role.cs                             # Entity Role (RoleId, RoleName)
    │   │   └── Address.cs                          # Entity Address (AddressId, UserId, ReceiverName, Phone, Địa chỉ)
    │   ├── Data/
    │   │   └── UserDbContext.cs                    # EF Core DbContext, cấu hình quan hệ, khóa ngoại, unique index
    │   ├── DTOs/                                   # Data Transfer Objects
    │   │   ├── Requests/
    │   │   │   ├── CreateUserRequestDto.cs         # DTO đăng ký user (FullName, Email, Password, RoleId)
    │   │   │   ├── UpdateUserRequestDto.cs         # DTO cập nhật thông tin user
    │   │   │   ├── CreateAddressRequestDto.cs      # DTO thêm địa chỉ mới
    │   │   │   ├── UpdateAddressRequestDto.cs      # DTO sửa địa chỉ
    │   │   │   └── UpdateDefaultAddressRequestDto.cs # DTO hỗ trợ đổi địa chỉ mặc định
    │   │   └── Responses/
    │   │       ├── UserResponseDto.cs              # DTO trả về thông tin user (ẩn mật khẩu)
    │   │       ├── RoleResponseDto.cs              # DTO trả về thông tin Role
    │   │       └── AddressResponseDto.cs           # DTO trả về địa chỉ người dùng
    │   ├── Repositories/                           # Tầng giao tiếp dữ liệu (Data Access)
    │   │   ├── Interfaces/
    │   │   │   ├── IUserRepository.cs              # Interface CRUD User
    │   │   │   ├── IRoleRepository.cs              # Interface đọc Role
    │   │   │   └── IAddressRepository.cs           # Interface CRUD Address
    │   │   ├── UserRepository.cs                   # Cài đặt IUserRepository bằng EF Core
    │   │   ├── RoleRepository.cs                   # Cài đặt IRoleRepository bằng EF Core
    │   │   └── AddressRepository.cs                # Cài đặt IAddressRepository bằng EF Core
    │   ├── Services/                               # Tầng xử lý nghiệp vụ (Business Logic)
    │   │   ├── Interfaces/
    │   │   │   ├── IUserService.cs                 # Định nghĩa nghiệp vụ User (Hash pass, soft delete)
    │   │   │   ├── IRoleService.cs                 # Định nghĩa nghiệp vụ Role
    │   │   │   └── IAddressService.cs              # Định nghĩa nghiệp vụ Address (Mặc định tự động)
    │   │   ├── UserService.cs                      # Triển khai IUserService: băm BCrypt, kiểm tra email trùng
    │   │   ├── RoleService.cs                      # Triển khai IRoleService
    │   │   └── AddressService.cs                   # Triển khai IAddressService: logic địa chỉ mặc định
    │   └── Controllers/                            # Tầng tiếp nhận HTTP Request (API Endpoints)
    │       ├── UserController.cs                   # Endpoints: `/api/users` (CRUD, by-email)
    │       ├── RoleController.cs                   # Endpoints: `/api/roles` (Lấy danh sách, lấy theo ID)
    │       ├── AddressController.cs                # Endpoints: `/api/addresses` (CRUD, set default)
    │       └── WeatherForecastController.cs        # Template mặc định
    │
    ├── ProductService/                             # DỊCH VỤ SẢN PHẨM & DANH MỤC (Port: 5071)
    │   ├── ProductService.csproj                   # Project cấu hình EF Core SqlServer, OpenApi
    │   ├── Program.cs                              # Đăng ký DI, cấu hình IgnoreCycles cho JSON
    │   ├── appsettings.json                        # Chuỗi kết nối ProductDB
    │   ├── appsettings.Development.json            # Cấu hình môi trường dev
    │   ├── Dockerfile                              # Kịch bản Docker ProductService
    │   ├── ProductService.http                     # Request test HTTP
    │   ├── WeatherForecast.cs                      # Template mặc định
    │   ├── Properties/
    │   │   └── launchSettings.json                 # Cấu hình cổng HTTP 5071 / HTTPS 7117
    │   ├── Models/
    │   │   ├── Category.cs                         # Entity Category (CategoryId, CategoryName, Description)
    │   │   └── Product.cs                          # Entity Product (ProductId, CategoryId, Name, Price, Stock)
    │   ├── Data/
    │   │   └── ProductDbContext.cs                 # EF Core DbContext, Check constraints Price > 0, Stock >= 0
    │   ├── DTOs/
    │   │   ├── Category/
    │   │   │   ├── CreateCategoryRequest.cs        # DTO tạo danh mục
    │   │   │   └── UpdateCategoryRequest.cs        # DTO cập nhật danh mục
    │   │   └── Product/
    │   │       ├── CreateProductRequest.cs         # DTO tạo sản phẩm mới
    │   │       └── UpdateProductRequest.cs         # DTO cập nhật sản phẩm
    │   ├── Repositories/
    │   │   ├── Interfaces/
    │   │   │   ├── ICategoryRepository.cs          # Interface CRUD Category
    │   │   │   └── IProductRepository.cs           # Interface CRUD Product
    │   │   ├── CategoryRepository.cs               # Cài đặt ICategoryRepository
    │   │   └── ProductRepository.cs                # Cài đặt IProductRepository
    │   ├── Services/
    │   │   ├── Interfaces/
    │   │   │   ├── ICategoryService.cs             # Định nghĩa nghiệp vụ danh mục
    │   │   │   └── IProductService.cs              # Định nghĩa nghiệp vụ sản phẩm
    │   │   ├── CategoryService.cs                  # Triển khai ICategoryService: kiểm tra sản phẩm trước khi xóa
    │   │   └── ProductService.cs                   # Triển khai IProductService: validate giá, tồn kho, category
    │   └── Controllers/
    │       ├── CategoryController.cs               # Endpoints: `/api/categories` (CRUD)
    │       ├── ProductController.cs                # Endpoints: `/api/products` (CRUD)
    │       └── WeatherForecastController.cs        # Template mặc định
    │
    ├── CartService/                                # DỊCH VỤ GIỎ HÀNG (Port: 5290)
    │   ├── CartService.csproj                      # Project cấu hình EF Core SqlServer, OpenApi
    │   ├── Program.cs                              # Đăng ký DI Repositories & Services, CartDbContext
    │   ├── appsettings.json                        # Chuỗi kết nối CartDB
    │   ├── appsettings.Development.json            # Cấu hình môi trường dev
    │   ├── Dockerfile                              # Kịch bản Docker CartService
    │   ├── CartService.http                        # Request test HTTP
    │   ├── WeatherForecast.cs                      # Template mặc định
    │   ├── Properties/
    │   │   └── launchSettings.json                 # Cấu hình cổng HTTP 5290 / HTTPS 7195
    │   ├── Models/
    │   │   ├── Cart.cs                             # Entity Cart (CartId, UserId, IsActive, CreatedAt, UpdatedAt)
    │   │   └── CartItem.cs                         # Entity CartItem (CartItemId, CartId, ProductId, Quantity)
    │   ├── Data/
    │   │   └── CartDbContext.cs                    # EF Core DbContext, Index UserId, Unique (CartId, ProductId)
    │   ├── DTOs/Cart/
    │   │   ├── AddToCartRequest.cs                 # DTO thêm sản phẩm vào giỏ (ProductId, Quantity)
    │   │   ├── UpdateCartItemRequest.cs            # DTO cập nhật số lượng sản phẩm trong giỏ
    │   │   ├── CartResponse.cs                     # DTO trả về thông tin toàn bộ giỏ hàng
    │   │   └── CartItemResponse.cs                 # DTO thông tin từng item trong giỏ
    │   ├── Repositories/
    │   │   ├── Interfaces/
    │   │   │   └── ICartRepository.cs              # Interface truy xuất Cart & CartItem
    │   │   └── Implementations/
    │   │       └── CartRepository.cs               # Triển khai ICartRepository qua EF Core
    │   ├── Services/
    │   │   ├── Interfaces/
    │   │   │   └── ICartService.cs                 # Định nghĩa nghiệp vụ giỏ hàng
    │   │   └── Implementations/
    │   │       └── CartService.cs                  # Triển khai ICartService: GetOrCreateCart, gộp số lượng
    │   └── Controllers/
    │       ├── CartController.cs                   # Endpoints: `/api/carts` (GET cart, add/update/delete items, clear)
    │       └── WeatherForecastController.cs        # Template mặc định
    │
    ├── OrderService/                               # DỊCH VỤ ĐƠN HÀNG (Port: 5218)
    │   ├── OrderService.csproj                     # Project cấu hình EF Core SqlServer, OpenApi
    │   ├── Program.cs                              # Đăng ký DI, cấu hình JsonStringEnumConverter cho OrderStatus
    │   ├── appsettings.json                        # Chuỗi kết nối OrderDB
    │   ├── appsettings.Development.json            # Cấu hình môi trường dev
    │   ├── Dockerfile                              # Kịch bản Docker OrderService
    │   ├── OrderService.http                       # Request test HTTP
    │   ├── WeatherForecast.cs                      # Template mặc định
    │   ├── Properties/
    │   │   └── launchSettings.json                 # Cấu hình cổng HTTP 5218 / HTTPS 7130
    │   ├── Models/
    │   │   ├── Order.cs                            # Entity Order (OrderId, UserId, TotalAmount, ShippingAddress...)
    │   │   ├── OrderItem.cs                        # Entity OrderItem (OrderItemId, OrderId, ProductId, Subtotal...)
    │   │   └── OrderStatus.cs                      # Enum trạng thái đơn hàng (PendingPayment, Confirmed, Cancelled...)
    │   ├── Data/
    │   │   └── OrderDbContext.cs                   # EF Core DbContext, ánh xạ Subtotal computed stored column
    │   ├── DTOs/Order/
    │   │   ├── CreateOrderRequest.cs               # DTO tạo đơn hàng (UserId, ReceiverName, Phone, OrderItems)
    │   │   ├── CreateOrderItemRequest.cs           # DTO từng món trong đơn tạo mới
    │   │   ├── UpdateOrderRequest.cs               # DTO cập nhật thông tin giao hàng khi đơn chưa xác nhận
    │   │   ├── UpdateOrderStatusRequest.cs         # DTO chuyển trạng thái đơn hàng
    │   │   ├── OrderResponse.cs                    # DTO trả về chi tiết đơn hàng
    │   │   └── OrderItemResponse.cs                # DTO trả về chi tiết món hàng trong đơn
    │   ├── Repositories/
    │   │   ├── Interfaces/
    │   │   │   └── IOrderRepository.cs             # Interface truy xuất Order & OrderItems
    │   │   └── OrderRepository.cs                  # Cài đặt IOrderRepository
    │   ├── Services/
    │   │   ├── Interfaces/
    │   │   │   └── IOrderService.cs                # Định nghĩa nghiệp vụ Order & State Machine
    │   │   └── OrderService.cs                     # Triển khai IOrderService: kiểm tra chuyển trạng thái, hủy đơn
    │   └── Controllers/
    │       ├── OrderController.cs                  # Endpoints: `/api/orders` (CRUD, status, cancel)
    │       └── WeatherForecastController.cs        # Template mặc định
    │
    └── PaymentService/                             # DỊCH VỤ THANH TOÁN (Port: 5151)
        ├── PaymentService.csproj                   # Project cấu hình EF Core SqlServer, OpenApi
        ├── Program.cs                              # Đăng ký DI, cấu hình JsonStringEnumConverter cho Enums
        ├── appsettings.json                        # Chuỗi kết nối PaymentDB
        ├── appsettings.Development.json            # Cấu hình môi trường dev
        ├── Dockerfile                              # Kịch bản Docker PaymentService
        ├── PaymentService.http                     # Request test HTTP
        ├── WeatherForecast.cs                      # Template mặc định
        ├── Properties/
        │   └── launchSettings.json                 # Cấu hình cổng HTTP 5151 / HTTPS 7013
        ├── Models/
        │   ├── Payment.cs                          # Entity Payment (PaymentId, OrderId, Amount, Method, Status...)
        │   └── Enums/
        │       ├── PaymentMethod.cs                # Enum phương thức thanh toán: COD, Simulation
        │       └── PaymentStatus.cs                # Enum trạng thái: Unpaid, Pending, Paid, Failed, Refunded
        ├── Data/
        │   └── PaymentDbContext.cs                 # EF Core DbContext, Unique Index OrderId, Filtered Index TransactionCode
        ├── DTOs/Payment/
        │   ├── CreatePaymentRequest.cs             # DTO tạo thanh toán (OrderId, Amount, PaymentMethod)
        │   ├── UpdatePaymentStatusRequest.cs       # DTO cập nhật trạng thái & TransactionCode
        │   └── PaymentResponse.cs                  # DTO trả về thông tin giao dịch
        ├── Repositories/
        │   ├── Interfaces/
        │   │   └── IPaymentRepository.cs           # Interface truy xuất bảng Payments
        │   └── Implementations/
        │       └── PaymentRepository.cs            # Cài đặt IPaymentRepository qua EF Core
        ├── Services/
        │   ├── Interfaces/
        │   │   └── IPaymentService.cs              # Định nghĩa nghiệp vụ kiểm soát thanh toán
        │   └── Implementations/
        │       └── PaymentService.cs               # Triển khai IPaymentService: logic CanTransition
        └── Controllers/
            ├── PaymentController.cs                # Endpoints: `/api/payments` (by-order, create, update status)
            └── WeatherForecastController.cs        # Template mặc định
```

---

## 5. BẢN ĐỒ CHI TIẾT TẤT CẢ API ENDPOINTS

### 5.1. UserService (`http://localhost:5104`)

| HTTP Method | Route | Request Body | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/users` | Không | Lấy danh sách toàn bộ người dùng |
| `GET` | `/api/users/{id}` | Không | Lấy thông tin chi tiết một người dùng theo User ID |
| `GET` | `/api/users/by-email?email=...` | Query param: `email` | Tìm người dùng theo địa chỉ email |
| `POST` | `/api/users` | `CreateUserRequestDto` | Đăng ký/Tạo mới user (tự động băm mật khẩu bằng BCrypt) |
| `PUT` | `/api/users/{id}` | `UpdateUserRequestDto` | Cập nhật thông tin họ tên, email của người dùng |
| `DELETE` | `/api/users/{id}` | Không | Xóa mềm tài khoản người dùng (`IsActive = false`) |
| `GET` | `/api/roles` | Không | Lấy danh sách tất cả các Role (Admin, Customer) |
| `GET` | `/api/roles/{id}` | Không | Lấy thông tin Role theo ID |
| `GET` | `/api/addresses/user/{userId}` | Không | Lấy danh sách tất cả địa chỉ nhận hàng của một người dùng |
| `GET` | `/api/addresses/{id}` | Không | Lấy chi tiết một địa chỉ theo ID |
| `POST` | `/api/addresses` | `CreateAddressRequestDto` | Thêm địa chỉ mới (tự động xử lý địa chỉ mặc định) |
| `PUT` | `/api/addresses/{id}` | `UpdateAddressRequestDto` | Cập nhật thông tin địa chỉ |
| `PATCH`| `/api/addresses/{id}/default` | Không | Thiết lập một địa chỉ làm địa chỉ mặc định |
| `DELETE`| `/api/addresses/{id}` | Không | Xóa địa chỉ (tự động chuyển mặc định sang địa chỉ kế tiếp) |

### 5.2. ProductService (`http://localhost:5071`)

| HTTP Method | Route | Request Body | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/categories` | Không | Lấy toàn bộ danh mục sản phẩm |
| `GET` | `/api/categories/{id}` | Không | Lấy thông tin chi tiết danh mục theo ID |
| `POST` | `/api/categories` | `CreateCategoryRequest` | Tạo danh mục mới |
| `PUT` | `/api/categories/{id}` | `UpdateCategoryRequest` | Cập nhật thông tin danh mục |
| `DELETE` | `/api/categories/{id}` | Không | Xóa danh mục (chỉ xóa nếu không có sản phẩm liên kết) |
| `GET` | `/api/products` | Không | Lấy toàn bộ danh sách sản phẩm |
| `GET` | `/api/products/{id}` | Không | Lấy thông tin chi tiết sản phẩm theo ID |
| `POST` | `/api/products` | `CreateProductRequest` | Tạo sản phẩm mới (kiểm tra CategoryId hợp lệ, Giá > 0, Kho >= 0) |
| `PUT` | `/api/products/{id}` | `UpdateProductRequest` | Cập nhật thông tin sản phẩm |
| `DELETE` | `/api/products/{id}` | Không | Xóa sản phẩm khỏi hệ thống |

### 5.3. CartService (`http://localhost:5290`)

| HTTP Method | Route | Request Body | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/carts/{userId}` | Không | Lấy giỏ hàng đang hoạt động của User (tự tạo nếu chưa có) |
| `POST` | `/api/carts/{userId}/items` | `AddToCartRequest` | Thêm sản phẩm vào giỏ (nếu có rồi thì cộng dồn số lượng) |
| `PUT` | `/api/carts/{userId}/items/{productId}` | `UpdateCartItemRequest` | Điều chỉnh số lượng một sản phẩm trong giỏ hàng |
| `DELETE` | `/api/carts/{userId}/items/{productId}` | Không | Xóa một sản phẩm cụ thể ra khỏi giỏ hàng |
| `DELETE` | `/api/carts/{userId}` | Không | Xóa toàn bộ sản phẩm trong giỏ hàng (Clear Cart) |

### 5.4. OrderService (`http://localhost:5218`)

| HTTP Method | Route | Request Body | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/orders` | Không | Lấy danh sách tất cả đơn hàng hệ thống |
| `GET` | `/api/orders/{id}` | Không | Lấy chi tiết đơn hàng cùng các sản phẩm con (`OrderItems`) |
| `GET` | `/api/orders/user/{userId}` | Không | Lấy danh sách lịch sử đơn hàng của một người dùng |
| `POST` | `/api/orders` | `CreateOrderRequest` | Tạo đơn hàng mới (tự động tính tổng tiền, đặt trạng thái ban đầu) |
| `PUT` | `/api/orders/{id}` | `UpdateOrderRequest` | Sửa thông tin người nhận/địa chỉ (chỉ khi `PendingConfirmation`) |
| `PATCH`| `/api/orders/{id}/status` | `UpdateOrderStatusRequest`| Cập nhật trạng thái đơn (kiểm soát theo State Machine nghiêm ngặt) |
| `PATCH`| `/api/orders/{id}/cancel?userId=...` | Query param: `userId` | Khách hàng tự hủy đơn (chỉ khi đang chờ xử lý / xác nhận) |

### 5.5. PaymentService (`http://localhost:5151`)

| HTTP Method | Route | Request Body | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/payments` | Không | Lấy danh sách toàn bộ giao dịch thanh toán |
| `GET` | `/api/payments/{id}` | Không | Lấy thông tin thanh toán theo PaymentId |
| `GET` | `/api/payments/by-order/{orderId}` | Không | Lấy thông tin thanh toán gắn với đơn hàng `OrderId` |
| `POST` | `/api/payments` | `CreatePaymentRequest` | Khởi tạo giao dịch thanh toán cho đơn hàng (mỗi đơn chỉ có 1) |
| `PUT` | `/api/payments/{id}/status` | `UpdatePaymentStatusRequest` | Cập nhật trạng thái thanh toán (`Paid`, `Failed`, `Refunded`, ...) |

---

## 6. QUY TẮC NGHIỆP VỤ QUAN TRỌNG (KEY BUSINESS RULES)

1. **Không sử dụng Khóa ngoại vật lý liên Database:**
   - Cột `UserId` trong `CartDB` và `OrderDB`, cột `ProductId` trong `CartDB` và `OrderDB`, cột `OrderId` trong `PaymentDB` là các tham chiếu logic. Tính toàn vẹn được kiểm soát bởi tầng ứng dụng Backend / API Gateway / Event Broker, không dùng SQL Server Foreign Key xuyên database.
2. **Bảo mật mật khẩu:**
   - Mật khẩu lưu trong `Users.PasswordHash` luôn được băm qua hàm `BCrypt.Net.BCrypt.HashPassword`. Không bao giờ lưu hay trả về mật khẩu plain-text trên bất kỳ DTO response nào.
3. **Quản lý địa chỉ giao hàng:**
   - Mỗi người dùng chỉ được có tối đa 1 địa chỉ nhận hàng mặc định (`IsDefault = 1`). Quy tắc này được bảo đảm ở 2 tầng: tầng ứng dụng (`AddressService`) và tầng CSDL thông qua SQL Server Filtered Unique Index `UX_Addresses_OneDefaultPerUser`.
4. **Máy trạng thái đơn hàng (Order State Machine):**
   - Không được nhảy cóc trạng thái tùy tiện. Đơn hàng chỉ có thể tiến dần theo luồng hợp lệ:
     `PendingPayment` -> `PendingConfirmation` -> `Confirmed` -> `Shipping` -> `Completed`.
   - Chỉ được phép hủy đơn (`Cancelled`) khi đơn chưa bắt đầu giao hàng (`PendingPayment`, `PendingConfirmation`, `Confirmed`).
5. **Thanh toán duy nhất (Idempotent / 1-to-1 Payment):**
   - Mỗi đơn hàng chỉ tương ứng với 1 bản ghi trong `Payments` (`UX_Payments_OrderId`). Khi thanh toán thất bại, hệ thống cập nhật bản ghi thanh toán hiện có thay vì tạo thêm bản ghi mới gây trùng lặp doanh thu.

---

## 7. QUY TRÌNH TRẢI NGHIỆM TRÊN WEB (END-TO-END WEB JOURNEY)

Toàn bộ luồng nghiệp vụ trên giao diện web đã được kết nối thông suốt với CSDL qua API Gateway:

```
[Khách hàng]
     │
     ├── 1. Đăng ký / Đăng nhập (`login.html`)
     │      ├── Đăng ký: Tạo user mới -> băm mật khẩu BCrypt -> lưu `UserDB` -> tự động đăng nhập.
     │      ├── Đăng nhập: Kiểm tra email & trạng thái hoạt động -> lưu session `localStorage`.
     │      └── Có sẵn tài khoản mẫu: `2355010172@kientrucanhanoi.edu.vn`, `b@gmail.com`, `admin@shopmini.com`.
     │
     ├── 2. Khám phá sản phẩm (`index.html` & `products.html`)
     │      ├── Lấy danh mục từ `ProductService` (`/api/categories`).
     │      ├── Tìm kiếm từ khóa theo tên sản phẩm, lọc theo danh mục, sắp xếp giá tăng/giảm.
     │      └── Xem chi tiết sản phẩm (`product-detail.html`): kiểm tra tồn kho (`Stock`), số lượng.
     │
     ├── 3. Quản lý giỏ hàng (`cart.html`)
     │      ├── Tự động đồng bộ giỏ hàng theo User qua `CartService` (`/api/carts/{userId}`).
     │      ├── Tăng/giảm số lượng từng món, xóa từng món, hoặc làm trống giỏ hàng (`Clear Cart`).
     │      └── Cập nhật tổng tiền tức thời dựa trên giá thực tế.
     │
     ├── 4. Đặt hàng & Tạo thanh toán (`cart.html`)
     │      ├── Nhập tên người nhận, số điện thoại, địa chỉ giao hàng và chọn phương thức:
     │      │   - `COD`: Thanh toán khi nhận hàng (Đơn hàng chuyển `PendingConfirmation`).
     │      │   - `Simulation`: Mô phỏng thanh toán trực tuyến (Đơn hàng ở `PendingPayment`).
     │      ├── Gửi request tạo đơn đến `OrderService` (`POST /api/orders`).
     │      ├── Tạo bản ghi thanh toán tương ứng đến `PaymentService` (`POST /api/payments`).
     │      └── Tự động dọn sạch giỏ hàng (`DELETE /api/carts/{userId}`) sau khi đặt thành công.
     │
     └── 5. Theo dõi & Quản lý đơn hàng (`orders.html`)
            ├── Xem danh sách đơn hàng đã mua (`GET /api/orders/user/{userId}`).
            ├── Khách hàng có thể tự hủy đơn hàng (`PATCH /api/orders/{id}/cancel`).
            └── Với đơn `Simulation` chưa trả tiền: Bấm nút "Thanh toán ngay" để mô phỏng thanh toán thành công (`Paid`), tự động chuyển trạng thái đơn hàng sang `Confirmed`.

```

```
[Quản trị viên - Admin Dashboard (`admin.html`)]
     │
     ├── 1. Xác thực quyền Quản trị:
     │      ├── Đăng nhập qua tài khoản Admin (`admin@shopmini.com` / `123456`).
     │      └── Tự động nhận diện Role Admin, chuyển hướng thẳng vào `admin.html`.
     │
     ├── 2. Thống kê Doanh thu & Kinh doanh (KPIs):
     │      ├── Tổng doanh thu: Tổng tiền các đơn hàng hợp lệ (`Confirmed`, `Shipping`, `Completed`).
     │      ├── Đã thu tiền (Paid): Thống kê từ `PaymentService` theo các giao dịch thành công.
     │      ├── Số lượng sản phẩm đã bán: Tổng hợp chi tiết từ từng món trong các đơn hàng.
     │      └── Tổng số lượng đơn hàng, số đơn chờ xử lý, số lượng mẫu mã và tổng tồn kho.
     │
     ├── 3. Quản lý Sản phẩm Kho hàng (CRUD):
     │      ├── Xem danh sách sản phẩm, tìm kiếm theo tên thời gian thực.
     │      ├── Thêm sản phẩm mới (Tên, Danh mục, Giá bán, Tồn kho, Ảnh, Mô tả) -> gọi `POST /api/products`.
     │      ├── Chỉnh sửa sản phẩm -> gọi `PUT /api/products/{id}`.
     │      └── Xóa sản phẩm khỏi hệ thống -> gọi `DELETE /api/products/{id}`.
     │
     ├── 4. Tình trạng Đơn hàng & State Machine:
     │      ├── Xem toàn bộ đơn hàng trong hệ thống: Mã đơn, Khách hàng, SĐT, Địa chỉ, Danh sách món, Tổng tiền, Thanh toán.
     │      ├── Bấm "Chi tiết" hoặc bấm vào Mã đơn để mở Modal xem tiến trình trực quan (Order Stepper: Chờ TT -> Chờ xác nhận -> Đã xác nhận -> Đang giao -> Hoàn thành).
     │      └── Cập nhật tình trạng đơn hàng trực tiếp theo State Machine:
     │          - `PendingPayment` -> `PendingConfirmation` hoặc `Cancelled`
     │          - `PendingConfirmation` -> `Confirmed` hoặc `Cancelled`
     │          - `Confirmed` -> `Shipping` hoặc `Cancelled`
     │          - `Shipping` -> `Completed`
     │
     └── 5. Bảng xếp hạng Sản phẩm đã bán (Top Sales):
            └── Tổng hợp bảng xếp hạng Top sản phẩm bán chạy nhất kèm số lượng đã bán và doanh thu đóng góp.
```

---

## 8. HIỆN TRẠNG VÀ CÁC HẠNG MỤC TIẾP THEO (CURRENT STATUS & ROADMAP)

### Hiện trạng đã sẵn sàng (100% Hoàn thành):
- **Toàn bộ 5 Microservices Backend** (`UserService`, `ProductService`, `CartService`, `OrderService`, `PaymentService`) đã hoàn thiện 100% mã nguồn từ Controller, Service, Repository, DTO đến DbContext.
- **API Gateway (`gateway/Gateway.Api`)** đã cấu hình hoàn chỉnh Ocelot Reverse Proxy, mở CORS `AllowAll`, định tuyến đầy đủ toàn bộ endpoint đến 5 microservices.
- **Frontend Client (`frontend/`)** đã hoàn thiện 100% các trang HTML, CSS, Responsive và JavaScript kết nối đầy đủ các hàm Controller Backend thông qua API Gateway.
- **Cơ sở dữ liệu SQL Server** có sẵn 3 file script: khởi tạo bảng (`init.sql`), dữ liệu mẫu (`seed.sql`), và bộ kiểm thử ràng buộc tự động (`test-constraints.sql`).
- **Biên dịch hệ thống:** Solution `ShopMini.sln` biên dịch sạch (0 errors, 0 warnings).

### Hạng mục tiếp theo (Tùy chọn nâng cao):
1. **Container hóa Docker:** Hoàn thiện `docker-compose.yml` để đóng gói và triển khai trên môi trường Docker container đa nền tảng.
2. **Kiểm thử E2E & Tự Merge:** Sau khi chạy thử toàn bộ luồng trên local thành công, tiến hành merge nhánh vào GitHub theo quy trình của nhóm.
