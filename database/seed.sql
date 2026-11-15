/*
    seed.sql - Du lieu mau thuc te cho Mini E-commerce (ShopMini)
    Chay sau khi da chay init.sql va tao du 5 database.
    Script co the chay lai: cac ban ghi mau duoc nhan dien bang ID/email co dinh.
*/

SET NOCOUNT ON;

------------------------------------------------------------
-- 1. USERDB: Roles, Users, Addresses
------------------------------------------------------------
USE UserDB;
GO

-- User 1: Khach hang Pham Ngoc Son
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'2355010172@kientrucanhanoi.edu.vn')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000001',
        N'Phạm Ngọc Sơn', N'2355010172@kientrucanhanoi.edu.vn', N'$2a$11$qN7YJ/sJ9vK7p8m8K9l0u.x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m', -- Hash BCrypt cho mat khau '123456'
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Customer';
END;
GO

-- User 2: Khach hang Tran Van B
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'b@gmail.com')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000002',
        N'Trần Văn B', N'b@gmail.com', N'$2a$11$qN7YJ/sJ9vK7p8m8K9l0u.x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m',
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Customer';
END;
GO

-- User 3: Administrator
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'admin@shopmini.com')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000003',
        N'Administrator', N'admin@shopmini.com', N'$2a$11$qN7YJ/sJ9vK7p8m8K9l0u.x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m',
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Admin';
END;
GO

-- Dia chi giao hang mac dinh cua Pham Ngoc Son
IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE AddressId = 'A2000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO dbo.Addresses
        (AddressId, UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A2000000-0000-0000-0000-000000000001',
         'A1000000-0000-0000-0000-000000000001',
         N'Phạm Ngọc Sơn', '0988777666', N'Hà Nội', N'Hà Đông',
         N'Mô Lao', N'Trường Đại học Kiến trúc Hà Nội', 1);
END;

-- Dia chi khac cua Pham Ngoc Son
IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE AddressId = 'A2000000-0000-0000-0000-000000000003')
BEGIN
    INSERT INTO dbo.Addresses
        (AddressId, UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A2000000-0000-0000-0000-000000000003',
         'A1000000-0000-0000-0000-000000000001',
         N'Phạm Ngọc Sơn', '0988777666', N'Hà Nội', N'Nam Từ Liêm',
         N'Mỹ Đình 1', N'Số 15 Lê Đức Thọ', 0);
END;

-- Dia chi giao hang mac dinh cua Tran Van B
IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE AddressId = 'A2000000-0000-0000-0000-000000000002')
BEGIN
    INSERT INTO dbo.Addresses
        (AddressId, UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A2000000-0000-0000-0000-000000000002',
         'A1000000-0000-0000-0000-000000000002',
         N'Trần Văn B', '0900000002', N'Hà Nội', N'Cầu Giấy',
         N'Dịch Vọng', N'Số 2, đường Xuân Thủy', 1);
END;
GO

------------------------------------------------------------
-- 2. PRODUCTDB: Categories, Products
------------------------------------------------------------
USE ProductDB;
GO

-- 2.1 Categories
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = 'B1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Categories (CategoryId, CategoryName, Description, IsActive)
    VALUES ('B1000000-0000-0000-0000-000000000001', N'Thiết bị ngoại vi & Máy tính', N'Bàn phím cơ, chuột không dây, phụ kiện PC', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = 'B1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Categories (CategoryId, CategoryName, Description, IsActive)
    VALUES ('B1000000-0000-0000-0000-000000000002', N'Phụ kiện công nghệ', N'Sạc cáp, mic thu âm, tai nghe không dây', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = 'B1000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.Categories (CategoryId, CategoryName, Description, IsActive)
    VALUES ('B1000000-0000-0000-0000-000000000003', N'Thời trang Streetwear', N'Áo thun, hoodie, quần áo Local Brand', 1);
GO

-- 2.2 Products
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000001',
         'B1000000-0000-0000-0000-000000000001',
         N'Bàn phím cơ Aula F75', N'Bàn phím cơ layout 75%, 3 chế độ kết nối, switch Reaper linear gõ cực êm.',
         N'/images/aula-f75.jpg', 850000, 50, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000002',
         'B1000000-0000-0000-0000-000000000001',
         N'Chuột không dây MCHOSE S9 PRO', N'Chuột gaming không dây siêu nhẹ, cảm biến PAW3395 chính xác.',
         N'/images/mchose-s9.jpg', 650000, 30, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000003',
         'B1000000-0000-0000-0000-000000000002',
         N'Củ sạc InfinityLab 20W USB-C', N'Củ sạc nhanh 20W chuẩn Power Delivery, thiết kế nhỏ gọn an toàn.',
         N'/images/infinitylab-20w.jpg', 250000, 100, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000004')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000004',
         'B1000000-0000-0000-0000-000000000002',
         N'Bộ Micro thu âm không dây DJI Mic 2', N'Micro thu âm chuyên nghiệp cho vlogging, chống ồn thông minh AI.',
         N'/images/dji-mic-2.jpg', 6900000, 15, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000005')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000005',
         'B1000000-0000-0000-0000-000000000003',
         N'Áo thun Oversize DirtyCoins Logo', N'Áo thun chất liệu 100% Cotton định lượng dày dặn, form dáng rộng thoải mái.',
         N'/images/dirtycoins-tee.jpg', 350000, 80, 1);
GO

------------------------------------------------------------
-- 3. CARTDB: Carts, CartItems
------------------------------------------------------------
USE CartDB;
GO

-- Gio hang hoat dong cua Pham Ngoc Son
IF NOT EXISTS (SELECT 1 FROM dbo.Carts WHERE CartId = 'C1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Carts (CartId, UserId, IsActive)
    VALUES ('C1000000-0000-0000-0000-000000000001',
            'A1000000-0000-0000-0000-000000000001', 1);
GO

-- Cac san pham trong gio hang cua Son
IF NOT EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartItemId = 'C2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.CartItems (CartItemId, CartId, ProductId, Quantity)
    VALUES ('C2000000-0000-0000-0000-000000000001',
            'C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000001', 1); -- 1 Bàn phím Aula F75

IF NOT EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartItemId = 'C2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.CartItems (CartItemId, CartId, ProductId, Quantity)
    VALUES ('C2000000-0000-0000-0000-000000000002',
            'C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000005', 2); -- 2 Áo thun DirtyCoins
GO

------------------------------------------------------------
-- 4. ORDERDB: Orders, OrderItems
------------------------------------------------------------
USE OrderDB;
GO

-- Don hang 1: Pham Ngoc Son dat mua Chuot MCHOSE & Sac InfinityLab (Trang thai: PendingConfirmation)
IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE OrderId = 'D1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Orders
        (OrderId, UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('D1000000-0000-0000-0000-000000000001',
         'A1000000-0000-0000-0000-000000000001',
         900000, N'Phạm Ngọc Sơn', '0988777666',
         N'Trường Đại học Kiến trúc Hà Nội, Mỗ Lao, Hà Đông, Hà Nội',
         N'PendingConfirmation');

-- Don hang 2: Tran Van B dat mua DJI Mic 2 (Trang thai: Completed)
IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE OrderId = 'D1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Orders
        (OrderId, UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('D1000000-0000-0000-0000-000000000002',
         'A1000000-0000-0000-0000-000000000002',
         6900000, N'Trần Văn B', '0900000002',
         N'Số 2, đường Xuân Thủy, Dịch Vọng, Cầu Giấy, Hà Nội',
         N'Completed');
GO

-- Chi tiet Don hang 1
IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000001',
         'D1000000-0000-0000-0000-000000000001',
         'B2000000-0000-0000-0000-000000000002',
         N'Chuột không dây MCHOSE S9 PRO', 650000, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000002',
         'D1000000-0000-0000-0000-000000000001',
         'B2000000-0000-0000-0000-000000000003',
         N'Củ sạc InfinityLab 20W USB-C', 250000, 1);

-- Chi tiet Don hang 2
IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000003',
         'D1000000-0000-0000-0000-000000000002',
         'B2000000-0000-0000-0000-000000000004',
         N'Bộ Micro thu âm không dây DJI Mic 2', 6900000, 1);
GO

------------------------------------------------------------
-- 5. PAYMENTDB: Payments
------------------------------------------------------------
USE PaymentDB;
GO

-- Thanh toan cho Don hang 1 (COD, Unpaid)
IF NOT EXISTS (SELECT 1 FROM dbo.Payments WHERE PaymentId = 'E1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Payments
        (PaymentId, OrderId, Amount, Method, Status, TransactionCode)
    VALUES
        ('E1000000-0000-0000-0000-000000000001',
         'D1000000-0000-0000-0000-000000000001',
         900000, N'COD', N'Unpaid', NULL);

-- Thanh toan cho Don hang 2 (Simulation, Paid)
IF NOT EXISTS (SELECT 1 FROM dbo.Payments WHERE PaymentId = 'E1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Payments
        (PaymentId, OrderId, Amount, Method, Status, TransactionCode)
    VALUES
        ('E1000000-0000-0000-0000-000000000002',
         'D1000000-0000-0000-0000-000000000002',
         6900000, N'Simulation', N'Paid', N'SHOPMINI-TXN-20261028');
GO

------------------------------------------------------------
-- 6. KIEM TRA: Dem so ban ghi trong tung bang
------------------------------------------------------------
SELECT N'UserDB' AS DatabaseName, N'Roles' AS TableName, COUNT_BIG(*) AS TotalRows FROM UserDB.dbo.Roles
UNION ALL SELECT N'UserDB', N'Users', COUNT_BIG(*) FROM UserDB.dbo.Users
UNION ALL SELECT N'UserDB', N'Addresses', COUNT_BIG(*) FROM UserDB.dbo.Addresses
UNION ALL SELECT N'ProductDB', N'Categories', COUNT_BIG(*) FROM ProductDB.dbo.Categories
UNION ALL SELECT N'ProductDB', N'Products', COUNT_BIG(*) FROM ProductDB.dbo.Products
UNION ALL SELECT N'CartDB', N'Carts', COUNT_BIG(*) FROM CartDB.dbo.Carts
UNION ALL SELECT N'CartDB', N'CartItems', COUNT_BIG(*) FROM CartDB.dbo.CartItems
UNION ALL SELECT N'OrderDB', N'Orders', COUNT_BIG(*) FROM OrderDB.dbo.Orders
UNION ALL SELECT N'OrderDB', N'OrderItems', COUNT_BIG(*) FROM OrderDB.dbo.OrderItems
UNION ALL SELECT N'PaymentDB', N'Payments', COUNT_BIG(*) FROM PaymentDB.dbo.Payments
ORDER BY DatabaseName, TableName;
GO