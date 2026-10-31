/*
Tập lệnh khởi tạo cơ sở dữ liệu
SQL Server
Tạo 5 cơ sở dữ liệu và các bảng của chúng.
Ghi chú:
- ID là UNIQUEIDENTIFIER với giá trị mặc định là NEWSEQUENTIALID().
- Mối quan hệ giữa các bảng trong cùng một cơ sở dữ liệu sử dụng khóa ngoại (FOREIGN KEY).
- Các tham chiếu chéo cơ sở dữ liệu (UserId, ProductId, OrderId) cố ý
không sử dụng ràng buộc khóa ngoại.
- Mỗi đơn hàng có một bản ghi thanh toán; thử lại việc cập nhật bản ghi đó.
- Trường UpdatedAt phải được ứng dụng làm mới khi một hàng được cập nhật.
*/

-- =========================================================
-- 1. Tạo các Database
-- =========================================================
IF DB_ID(N'UserDB') IS NULL CREATE DATABASE UserDB;
GO
IF DB_ID(N'ProductDB') IS NULL CREATE DATABASE ProductDB;
GO
IF DB_ID(N'CartDB') IS NULL CREATE DATABASE CartDB;
GO
IF DB_ID(N'OrderDB') IS NULL CREATE DATABASE OrderDB;
GO
IF DB_ID(N'PaymentDB') IS NULL CREATE DATABASE PaymentDB;
GO

-- =========================================================
-- 2. Tạo các bảng UserDB
-- =========================================================
USE UserDB;
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        RoleId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Roles PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        RoleName NVARCHAR(50) NOT NULL
            CONSTRAINT UQ_Roles_RoleName UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Users PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Users_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT FK_Users_Roles
            FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Addresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Addresses
    (
        AddressId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Addresses PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        ReceiverName NVARCHAR(100) NOT NULL,
        Phone VARCHAR(15) NOT NULL,
        Province NVARCHAR(100) NOT NULL,
        District NVARCHAR(100) NOT NULL,
        Ward NVARCHAR(100) NOT NULL,
        DetailAddress NVARCHAR(255) NOT NULL,
        IsDefault BIT NOT NULL
            CONSTRAINT DF_Addresses_IsDefault DEFAULT (0),

        CONSTRAINT FK_Addresses_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
    );
END;
GO

-- Mỗi người dùng chỉ có thể có tối đa một địa chỉ mặc định.
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Addresses_OneDefaultPerUser'
      AND object_id = OBJECT_ID(N'dbo.Addresses')
)
BEGIN
    CREATE UNIQUE INDEX UX_Addresses_OneDefaultPerUser
        ON dbo.Addresses(UserId)
        WHERE IsDefault = 1;
END;
GO

-- Vai trò ban đầu. Có thể chạy nhiều lần một cách an toàn.
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Customer')
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Customer');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Admin')
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Admin');
GO

-- =========================================================
-- 3. Tạo các bảng ProductDB
-- =========================================================
USE ProductDB;
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Categories PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_Categories_IsActive DEFAULT (1),

        CONSTRAINT UQ_Categories_CategoryName UNIQUE (CategoryName)
    );
END;
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        ProductId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Products PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        CategoryId UNIQUEIDENTIFIER NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        ImageUrl NVARCHAR(500) NULL,
        Price DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL
            CONSTRAINT DF_Products_Stock DEFAULT (0),
        IsActive BIT NOT NULL
            CONSTRAINT DF_Products_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Products_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Products_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT FK_Products_Categories
            FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId),
        CONSTRAINT CK_Products_Price CHECK (Price > 0),
        CONSTRAINT CK_Products_Stock CHECK (Stock >= 0)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Products_CategoryId'
      AND object_id = OBJECT_ID(N'dbo.Products')
)
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
GO

-- =========================================================
-- 4. Tạo các bảng CartDB
-- =========================================================
USE CartDB;
GO

IF OBJECT_ID(N'dbo.Carts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Carts
    (
        CartId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Carts PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_Carts_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Carts_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Carts_UpdatedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

-- Mỗi người dùng chỉ có thể có một giỏ hàng đang hoạt động; các giỏ hàng cũ có thể được lưu lại để xem lịch sử.
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Carts_OneActiveCartPerUser'
      AND object_id = OBJECT_ID(N'dbo.Carts')
)
BEGIN
    CREATE UNIQUE INDEX UX_Carts_OneActiveCartPerUser
        ON dbo.Carts(UserId)
        WHERE IsActive = 1;
END;
GO

IF OBJECT_ID(N'dbo.CartItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CartItems
    (
        CartItemId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_CartItems PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        CartId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        Quantity INT NOT NULL,

        CONSTRAINT FK_CartItems_Carts
            FOREIGN KEY (CartId) REFERENCES dbo.Carts(CartId),
        CONSTRAINT UQ_CartItems_Cart_Product UNIQUE (CartId, ProductId),
        CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0)
    );
END;
GO

-- ProductId cố ý không có khóa ngoại: ProductDB thuộc về một dịch vụ khác.
-- UserId cố ý không có khóa ngoại: UserDB thuộc về một dịch vụ khác.

-- =========================================================
-- 5. tạo các bảng OrderDB
-- =========================================================
USE OrderDB;
GO

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        OrderId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Orders PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        ReceiverName NVARCHAR(100) NOT NULL,
        Phone VARCHAR(15) NOT NULL,
        ShippingAddress NVARCHAR(500) NOT NULL,
        Status NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Orders_Status DEFAULT (N'PendingConfirmation'),
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Orders_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0),
        CONSTRAINT CK_Orders_Status CHECK
        (
            Status IN
            (
                N'PendingPayment',
                N'PaymentFailed',
                N'PendingConfirmation',
                N'Confirmed',
                N'Shipping',
                N'Completed',
                N'Cancelled'
            )
        )
    );
END;
GO

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        OrderItemId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_OrderItems PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        OrderId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        Subtotal AS CONVERT(DECIMAL(18,2), UnitPrice * Quantity) PERSISTED,

        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId),
        CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Orders_UserId'
      AND object_id = OBJECT_ID(N'dbo.Orders')
)
    CREATE INDEX IX_Orders_UserId ON dbo.Orders(UserId);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_OrderItems_OrderId'
      AND object_id = OBJECT_ID(N'dbo.OrderItems')
)
    CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
GO

-- ProductId và UserId cố ý không có khóa ngoại liên cơ sở dữ liệu.

-- =========================================================
-- 6. Tạo các bảng PaymentDB
-- =========================================================
USE PaymentDB;
GO

IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments
    (
        PaymentId UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Payments PRIMARY KEY
            DEFAULT NEWSEQUENTIALID(),
        OrderId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Method NVARCHAR(30) NOT NULL,
        Status NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Payments_Status DEFAULT (N'Pending'),
        TransactionCode NVARCHAR(100) NULL,
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Payments_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Payments_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT CK_Payments_Amount CHECK (Amount >= 0),
        CONSTRAINT CK_Payments_Method CHECK
            (Method IN (N'COD', N'Simulation')),
        CONSTRAINT CK_Payments_Status CHECK
            (Status IN (N'Unpaid', N'Pending', N'Paid', N'Failed', N'Refunded'))
    );
END;
GO

-- Mã giao dịch, khi được cung cấp, phải là duy nhất.
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Payments_TransactionCode'
      AND object_id = OBJECT_ID(N'dbo.Payments')
)
BEGIN
    CREATE UNIQUE INDEX UX_Payments_TransactionCode
        ON dbo.Payments(TransactionCode)
        WHERE TransactionCode IS NOT NULL;
END;
GO

-- Mỗi đơn hàng có một bản ghi thanh toán trong dự án này.
-- Nếu thanh toán không thành công, bạn có thể thử lại bằng cách cập nhật bản ghi thanh toán hiện có.
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Payments_OrderId'
      AND object_id = OBJECT_ID(N'dbo.Payments')
)
    CREATE UNIQUE INDEX UX_Payments_OrderId ON dbo.Payments(OrderId);
GO

-- OrderId cố ý không có khóa ngoại: OrderDB thuộc về một dịch vụ khác.

-- =========================================================
-- 7. Kiểm tra các database và các bảng
-- =========================================================
SELECT name AS DatabaseName
FROM sys.databases
WHERE name IN (N'UserDB', N'ProductDB', N'CartDB', N'OrderDB', N'PaymentDB')
ORDER BY name;
GO

USE UserDB;
SELECT N'UserDB' AS DatabaseName, name AS TableName
FROM sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
UNION ALL
SELECT N'ProductDB', name FROM ProductDB.sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
UNION ALL
SELECT N'CartDB', name FROM CartDB.sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
UNION ALL
SELECT N'OrderDB', name FROM OrderDB.sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
UNION ALL
SELECT N'PaymentDB', name FROM PaymentDB.sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
ORDER BY DatabaseName, TableName;
GO
