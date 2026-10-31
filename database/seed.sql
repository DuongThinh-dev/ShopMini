/*
    seed.sql - Du lieu mau cho Mini E-commerce
    Chay sau khi da chay init.sql va tao du 5 database.
    Script co the chay lai: cac ban ghi mau duoc nhan dien bang ID/email co dinh.
*/

SET NOCOUNT ON;

------------------------------------------------------------
-- 1. USERDB: Roles, Users, Addresses
------------------------------------------------------------
USE UserDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'a@gmail.com')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000001',
        N'Nguyen Van A', N'a@gmail.com', N'123456',
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Customer';
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'b@gmail.com')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000002',
        N'Tran Van B', N'b@gmail.com', N'456789',
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Customer';
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'admin@gmail.com')
BEGIN
    INSERT INTO dbo.Users
        (UserId, FullName, Email, PasswordHash, RoleId, IsActive)
    SELECT
        'A1000000-0000-0000-0000-000000000003',
        N'Administrator', N'admin@gmail.com', N'123789',
        RoleId, 1
    FROM dbo.Roles WHERE RoleName = N'Admin';
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE AddressId = 'A2000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO dbo.Addresses
        (AddressId, UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A2000000-0000-0000-0000-000000000001',
         'A1000000-0000-0000-0000-000000000001',
         N'Nguyen Van A', '0900000001', N'Ha Noi', N'Nam Tu Liem',
         N'My Dinh 1', N'So 1, duong Demo', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE AddressId = 'A2000000-0000-0000-0000-000000000002')
BEGIN
    INSERT INTO dbo.Addresses
        (AddressId, UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A2000000-0000-0000-0000-000000000002',
         'A1000000-0000-0000-0000-000000000002',
         N'Tran Van B', '0900000002', N'Ha Noi', N'Cau Giay',
         N'Dich Vong', N'So 2, duong Demo', 1);
END;
GO

------------------------------------------------------------
-- 2. PRODUCTDB: Categories, Products
------------------------------------------------------------
USE ProductDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = 'B1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Categories (CategoryId, CategoryName, Description, IsActive)
    VALUES ('B1000000-0000-0000-0000-000000000001',
            N'Dien thoai', N'Dien thoai di dong', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = 'B1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Categories (CategoryId, CategoryName, Description, IsActive)
    VALUES ('B1000000-0000-0000-0000-000000000002',
            N'Phu kien', N'Phu kien dien tu', 1);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000001',
         'B1000000-0000-0000-0000-000000000001',
         N'Dien thoai Demo X', N'Dien thoai mau de kiem thu',
         NULL, 12000000, 20, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000002',
         'B1000000-0000-0000-0000-000000000002',
         N'Tai nghe Demo', N'Tai nghe phuc vu kiem thu gio hang',
         NULL, 500000, 50, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 'B2000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.Products
        (ProductId, CategoryId, ProductName, Description, ImageUrl, Price, Stock, IsActive)
    VALUES
        ('B2000000-0000-0000-0000-000000000003',
         'B1000000-0000-0000-0000-000000000002',
         N'Chuot khong day', N'Chuot may tinh khong day',
         NULL, 250000, 35, 1);
GO

------------------------------------------------------------
-- 3. CARTDB: Carts, CartItems
------------------------------------------------------------
USE CartDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Carts WHERE CartId = 'C1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Carts (CartId, UserId, IsActive)
    VALUES ('C1000000-0000-0000-0000-000000000001',
            'A1000000-0000-0000-0000-000000000001', 1);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartItemId = 'C2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.CartItems (CartItemId, CartId, ProductId, Quantity)
    VALUES ('C2000000-0000-0000-0000-000000000001',
            'C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000001', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartItemId = 'C2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.CartItems (CartItemId, CartId, ProductId, Quantity)
    VALUES ('C2000000-0000-0000-0000-000000000002',
            'C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000002', 2);
GO

------------------------------------------------------------
-- 4. ORDERDB: Orders, OrderItems
------------------------------------------------------------
USE OrderDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE OrderId = 'D1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Orders
        (OrderId, UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('D1000000-0000-0000-0000-000000000001',
         'A1000000-0000-0000-0000-000000000001',
         13000000, N'Nguyen Van A', '0900000001',
         N'So 1, duong Demo, My Dinh 1, Nam Tu Liem, Ha Noi',
         N'PendingConfirmation');

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE OrderId = 'D1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Orders
        (OrderId, UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('D1000000-0000-0000-0000-000000000002',
         'A1000000-0000-0000-0000-000000000002',
         500000, N'Tran Van B', '0900000002',
         N'So 2, duong Demo, Dich Vong, Cau Giay, Ha Noi',
         N'Completed');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000001',
         'D1000000-0000-0000-0000-000000000001',
         'B2000000-0000-0000-0000-000000000001',
         N'Dien thoai Demo X', 12000000, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000002',
         'D1000000-0000-0000-0000-000000000001',
         'B2000000-0000-0000-0000-000000000002',
         N'Tai nghe Demo', 500000, 2);

IF NOT EXISTS (SELECT 1 FROM dbo.OrderItems WHERE OrderItemId = 'D2000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.OrderItems
        (OrderItemId, OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES
        ('D2000000-0000-0000-0000-000000000003',
         'D1000000-0000-0000-0000-000000000002',
         'B2000000-0000-0000-0000-000000000002',
         N'Tai nghe Demo', 500000, 1);
GO

------------------------------------------------------------
-- 5. PAYMENTDB: Payments
------------------------------------------------------------
USE PaymentDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Payments WHERE PaymentId = 'E1000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Payments
        (PaymentId, OrderId, Amount, Method, Status, TransactionCode)
    VALUES
        ('E1000000-0000-0000-0000-000000000001',
         'D1000000-0000-0000-0000-000000000001',
         13000000, N'COD', N'Unpaid', NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Payments WHERE PaymentId = 'E1000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Payments
        (PaymentId, OrderId, Amount, Method, Status, TransactionCode)
    VALUES
        ('E1000000-0000-0000-0000-000000000002',
         'D1000000-0000-0000-0000-000000000002',
         500000, N'Simulation', N'Paid', N'DEMO-TXN-0002');
GO

------------------------------------------------------------
-- 6. KIEM TRA: dem so ban ghi trong tung bang
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

------------------------------------------------------------
-- 7. KIEM TRA: xem du lieu mau theo tung nghiep vu
------------------------------------------------------------
SELECT u.FullName, u.Email, r.RoleName, u.IsActive
FROM UserDB.dbo.Users AS u
JOIN UserDB.dbo.Roles AS r ON r.RoleId = u.RoleId
ORDER BY u.Email;

SELECT c.CategoryName, p.ProductName, p.Price, p.Stock, p.IsActive
FROM ProductDB.dbo.Products AS p
JOIN ProductDB.dbo.Categories AS c ON c.CategoryId = p.CategoryId
ORDER BY c.CategoryName, p.ProductName;

SELECT ca.UserId, ci.ProductId, p.ProductName, p.Price, ci.Quantity,
       CONVERT(DECIMAL(18,2), p.Price * ci.Quantity) AS LineTotal
FROM CartDB.dbo.Carts AS ca
JOIN CartDB.dbo.CartItems AS ci ON ci.CartId = ca.CartId
JOIN ProductDB.dbo.Products AS p ON p.ProductId = ci.ProductId
WHERE ca.CartId = 'C1000000-0000-0000-0000-000000000001';

SELECT o.OrderId, o.UserId, o.Status, o.TotalAmount,
       oi.ProductName, oi.UnitPrice, oi.Quantity, oi.Subtotal
FROM OrderDB.dbo.Orders AS o
JOIN OrderDB.dbo.OrderItems AS oi ON oi.OrderId = o.OrderId
ORDER BY o.CreatedAt, oi.ProductName;

SELECT OrderId, Amount, Method, Status, TransactionCode
FROM PaymentDB.dbo.Payments
ORDER BY CreatedAt;
GO
