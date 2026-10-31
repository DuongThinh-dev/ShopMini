/*
    test-constraints.sql
    Kiem thu cac rang buoc cua 5 Database Mini E-commerce.
    Dieu kien: da chay init.sql va seed.sql.
    Tat ca INSERT thu nghiem deu duoc ROLLBACK.
    Ket qua PASS nghia la SQL Server da tu choi du lieu khong hop le
    bang dung nhom loi du kien.
*/

SET NOCOUNT ON;

PRINT N'========== USERDB ==========';

USE UserDB;
GO

-- U01: PRIMARY KEY Roles.RoleId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Roles (RoleId, RoleName)
    SELECT RoleId, N'Role PK Test' FROM dbo.Roles WHERE RoleName = N'Customer';
    ROLLBACK;
    PRINT N'FAIL U01 - PRIMARY KEY cho phep trung RoleId';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 2627 PRINT N'PASS U01 - PRIMARY KEY da chan RoleId trung';
    ELSE PRINT CONCAT(N'FAIL U01 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U02: UNIQUE Roles.RoleName
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Roles (RoleName) VALUES (N'Customer');
    ROLLBACK;
    PRINT N'FAIL U02 - UNIQUE RoleName cho phep trung';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS U02 - UNIQUE RoleName da chan gia tri trung';
    ELSE PRINT CONCAT(N'FAIL U02 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U03: UNIQUE Users.Email
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleId)
    SELECT N'Test Duplicate', N'a@gmail.com', N'demo', RoleId
    FROM dbo.Roles WHERE RoleName = N'Customer';
    ROLLBACK;
    PRINT N'FAIL U03 - UNIQUE Email cho phep trung';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS U03 - UNIQUE Email da chan email trung';
    ELSE PRINT CONCAT(N'FAIL U03 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U04: FOREIGN KEY Users.RoleId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleId)
    VALUES (N'Test FK', N'fk-user-test@example.com', N'demo',
            'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF');
    ROLLBACK;
    PRINT N'FAIL U04 - FOREIGN KEY RoleId cho phep RoleId khong ton tai';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS U04 - FOREIGN KEY RoleId da chan tham chieu sai';
    ELSE PRINT CONCAT(N'FAIL U04 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U05: NOT NULL Users.FullName
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleId)
    SELECT NULL, N'null-name-test@example.com', N'demo', RoleId
    FROM dbo.Roles WHERE RoleName = N'Customer';
    ROLLBACK;
    PRINT N'FAIL U05 - NOT NULL FullName cho phep NULL';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 515 PRINT N'PASS U05 - NOT NULL FullName da chan NULL';
    ELSE PRINT CONCAT(N'FAIL U05 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U06: UNIQUE filtered index - mot dia chi mac dinh / user
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Addresses
        (UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('A1000000-0000-0000-0000-000000000001',
         N'Nguoi nhan test', '0900000099', N'Ha Noi', N'Quan test',
         N'Phuong test', N'Dia chi thu nghiem', 1);
    ROLLBACK;
    PRINT N'FAIL U06 - Cho phep mot user co hai dia chi mac dinh';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS U06 - Moi user chi co toi da mot dia chi mac dinh';
    ELSE PRINT CONCAT(N'FAIL U06 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- U07: FOREIGN KEY Addresses.UserId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Addresses
        (UserId, ReceiverName, Phone, Province, District, Ward, DetailAddress, IsDefault)
    VALUES
        ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
         N'Test FK', '0900000098', N'Ha Noi', N'Quan test',
         N'Phuong test', N'Dia chi thu nghiem', 0);
    ROLLBACK;
    PRINT N'FAIL U07 - FOREIGN KEY Addresses.UserId cho phep user khong ton tai';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS U07 - FOREIGN KEY Addresses.UserId da chan tham chieu sai';
    ELSE PRINT CONCAT(N'FAIL U07 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== PRODUCTDB ==========';

USE ProductDB;
GO

-- P01: UNIQUE Categories.CategoryName
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Categories (CategoryName) VALUES (N'Dien thoai');
    ROLLBACK;
    PRINT N'FAIL P01 - UNIQUE CategoryName cho phep trung';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS P01 - UNIQUE CategoryName da chan trung';
    ELSE PRINT CONCAT(N'FAIL P01 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- P02: FOREIGN KEY Products.CategoryId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Products (CategoryId, ProductName, Price, Stock)
    VALUES ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', N'Test FK Product', 1000, 1);
    ROLLBACK;
    PRINT N'FAIL P02 - FOREIGN KEY CategoryId cho phep danh muc khong ton tai';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS P02 - FOREIGN KEY CategoryId da chan tham chieu sai';
    ELSE PRINT CONCAT(N'FAIL P02 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- P03: CHECK Products.Price > 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Products (CategoryId, ProductName, Price, Stock)
    SELECT CategoryId, N'Test Negative Price', 0, 1
    FROM dbo.Categories WHERE CategoryName = N'Dien thoai';
    ROLLBACK;
    PRINT N'FAIL P03 - CHECK Price cho phep gia bang 0';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS P03 - CHECK Price da chan gia khong hop le';
    ELSE PRINT CONCAT(N'FAIL P03 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- P04: CHECK Products.Stock >= 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Products (CategoryId, ProductName, Price, Stock)
    SELECT CategoryId, N'Test Negative Stock', 1000, -1
    FROM dbo.Categories WHERE CategoryName = N'Dien thoai';
    ROLLBACK;
    PRINT N'FAIL P04 - CHECK Stock cho phep so luong am';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS P04 - CHECK Stock da chan so luong am';
    ELSE PRINT CONCAT(N'FAIL P04 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== CARTDB ==========';

USE CartDB;
GO

-- C01: UNIQUE filtered index - mot gio hang active / user
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Carts (UserId, IsActive)
    VALUES ('A1000000-0000-0000-0000-000000000001', 1);
    ROLLBACK;
    PRINT N'FAIL C01 - Cho phep hai gio hang dang hoat dong cho mot user';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS C01 - Moi user chi co mot gio hang dang hoat dong';
    ELSE PRINT CONCAT(N'FAIL C01 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- C02: FOREIGN KEY CartItems.CartId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.CartItems (CartId, ProductId, Quantity)
    VALUES ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
            'B2000000-0000-0000-0000-000000000001', 1);
    ROLLBACK;
    PRINT N'FAIL C02 - FOREIGN KEY CartId cho phep gio hang khong ton tai';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS C02 - FOREIGN KEY CartId da chan tham chieu sai';
    ELSE PRINT CONCAT(N'FAIL C02 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- C03: UNIQUE CartItems(CartId, ProductId)
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.CartItems (CartId, ProductId, Quantity)
    VALUES ('C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000001', 1);
    ROLLBACK;
    PRINT N'FAIL C03 - Cho phep mot san pham lap lai trong cung gio hang';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS C03 - UNIQUE CartId va ProductId da chan dong trung';
    ELSE PRINT CONCAT(N'FAIL C03 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- C04: CHECK CartItems.Quantity > 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.CartItems (CartId, ProductId, Quantity)
    VALUES ('C1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000003', 0);
    ROLLBACK;
    PRINT N'FAIL C04 - CHECK Quantity cho phep so luong bang 0';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS C04 - CHECK Quantity da chan so luong khong hop le';
    ELSE PRINT CONCAT(N'FAIL C04 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== ORDERDB ==========';

USE OrderDB;
GO

-- O01: CHECK Orders.TotalAmount >= 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Orders
        (UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('A1000000-0000-0000-0000-000000000001',
         -1, N'Test', '0900000097', N'Dia chi test', N'PendingConfirmation');
    ROLLBACK;
    PRINT N'FAIL O01 - CHECK TotalAmount cho phep so tien am';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS O01 - CHECK TotalAmount da chan so tien am';
    ELSE PRINT CONCAT(N'FAIL O01 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- O02: CHECK Orders.Status chi nhan cac trang thai da quy dinh
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Orders
        (UserId, TotalAmount, ReceiverName, Phone, ShippingAddress, Status)
    VALUES
        ('A1000000-0000-0000-0000-000000000001',
         1000, N'Test', '0900000096', N'Dia chi test', N'UnknownStatus');
    ROLLBACK;
    PRINT N'FAIL O02 - CHECK Status cho phep trang thai khong hop le';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS O02 - CHECK Status da chan trang thai la';
    ELSE PRINT CONCAT(N'FAIL O02 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- O03: FOREIGN KEY OrderItems.OrderId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
            'B2000000-0000-0000-0000-000000000001', N'Test FK', 1000, 1);
    ROLLBACK;
    PRINT N'FAIL O03 - FOREIGN KEY OrderId cho phep don hang khong ton tai';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS O03 - FOREIGN KEY OrderId da chan tham chieu sai';
    ELSE PRINT CONCAT(N'FAIL O03 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- O04: CHECK OrderItems.UnitPrice >= 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES ('D1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000003', N'Test Price', -1, 1);
    ROLLBACK;
    PRINT N'FAIL O04 - CHECK UnitPrice cho phep gia am';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS O04 - CHECK UnitPrice da chan gia am';
    ELSE PRINT CONCAT(N'FAIL O04 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- O05: CHECK OrderItems.Quantity > 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductName, UnitPrice, Quantity)
    VALUES ('D1000000-0000-0000-0000-000000000001',
            'B2000000-0000-0000-0000-000000000003', N'Test Quantity', 1000, 0);
    ROLLBACK;
    PRINT N'FAIL O05 - CHECK Quantity cho phep so luong 0';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS O05 - CHECK Quantity da chan so luong 0';
    ELSE PRINT CONCAT(N'FAIL O05 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== PAYMENTDB ==========';

USE PaymentDB;
GO

-- M01: CHECK Payments.Amount >= 0
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Payments (OrderId, Amount, Method, Status)
    VALUES ('D1000000-0000-0000-0000-000000000003',
            -1, N'COD', N'Unpaid');
    ROLLBACK;
    PRINT N'FAIL M01 - CHECK Amount cho phep so tien am';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS M01 - CHECK Amount da chan so tien am';
    ELSE PRINT CONCAT(N'FAIL M01 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- M02: CHECK Payments.Method
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Payments (OrderId, Amount, Method, Status)
    VALUES ('D1000000-0000-0000-0000-000000000003',
            1000, N'CreditCard', N'Unpaid');
    ROLLBACK;
    PRINT N'FAIL M02 - CHECK Method cho phep phuong thuc khong hop le';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS M02 - CHECK Method da chan phuong thuc la';
    ELSE PRINT CONCAT(N'FAIL M02 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- M03: CHECK Payments.Status
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Payments (OrderId, Amount, Method, Status)
    VALUES ('D1000000-0000-0000-0000-000000000003',
            1000, N'COD', N'UnknownStatus');
    ROLLBACK;
    PRINT N'FAIL M03 - CHECK Status cho phep trang thai khong hop le';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err = 547 PRINT N'PASS M03 - CHECK Status da chan trang thai la';
    ELSE PRINT CONCAT(N'FAIL M03 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- M04: UNIQUE Payments.OrderId
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Payments (OrderId, Amount, Method, Status)
    VALUES ('D1000000-0000-0000-0000-000000000001',
            13000000, N'COD', N'Unpaid');
    ROLLBACK;
    PRINT N'FAIL M04 - Cho phep mot OrderId co nhieu Payments';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS M04 - UNIQUE OrderId da chan thanh toan trung don';
    ELSE PRINT CONCAT(N'FAIL M04 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

-- M05: UNIQUE filtered index Payments.TransactionCode
DECLARE @err INT;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Payments (OrderId, Amount, Method, Status, TransactionCode)
    VALUES ('D1000000-0000-0000-0000-000000000003',
            1000, N'Simulation', N'Paid', N'DEMO-TXN-0002');
    ROLLBACK;
    PRINT N'FAIL M05 - Cho phep TransactionCode trung';
END TRY
BEGIN CATCH
    SET @err = ERROR_NUMBER();
    IF @@TRANCOUNT > 0 ROLLBACK;
    IF @err IN (2601, 2627) PRINT N'PASS M05 - UNIQUE TransactionCode da chan ma giao dich trung';
    ELSE PRINT CONCAT(N'FAIL M05 - Loi khong mong doi: ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== KIEM TRA DEFAULT ==========';

-- Kiem tra DEFAULT cua Users.IsActive va CreatedAt/UpdatedAt
USE UserDB;
GO
DECLARE @newUserId UNIQUEIDENTIFIER;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleId)
    SELECT N'Default Test', N'default-test@example.com', N'demo', RoleId
    FROM dbo.Roles WHERE RoleName = N'Customer';

    SELECT @newUserId = UserId
    FROM dbo.Users WHERE Email = N'default-test@example.com';

    IF EXISTS
    (
        SELECT 1 FROM dbo.Users
        WHERE UserId = @newUserId
          AND IsActive = 1
          AND CreatedAt IS NOT NULL
          AND UpdatedAt IS NOT NULL
    )
        PRINT N'PASS D01 - DEFAULT Users.IsActive, CreatedAt, UpdatedAt hoat dong';
    ELSE
        PRINT N'FAIL D01 - Gia tri DEFAULT Users khong dung';

    ROLLBACK;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT CONCAT(N'FAIL D01 - ', ERROR_MESSAGE());
END CATCH;
GO

-- Kiem tra DEFAULT cua Products.Stock va IsActive
USE ProductDB;
GO
DECLARE @newProductId UNIQUEIDENTIFIER;
BEGIN TRY
    BEGIN TRAN;
    INSERT INTO dbo.Products (CategoryId, ProductName, Price)
    SELECT CategoryId, N'Default Test Product', 1000
    FROM dbo.Categories WHERE CategoryName = N'Dien thoai';

    SELECT @newProductId = ProductId
    FROM dbo.Products WHERE ProductName = N'Default Test Product';

    IF EXISTS
    (
        SELECT 1 FROM dbo.Products
        WHERE ProductId = @newProductId AND Stock = 0 AND IsActive = 1
    )
        PRINT N'PASS D02 - DEFAULT Products.Stock va IsActive hoat dong';
    ELSE
        PRINT N'FAIL D02 - Gia tri DEFAULT Products khong dung';

    ROLLBACK;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT CONCAT(N'FAIL D02 - ', ERROR_MESSAGE());
END CATCH;
GO

PRINT N'========== HOAN TAT KIEM THU ==========';

-- Ghi chu:
-- UserId trong CartDB/OrderDB, ProductId trong CartDB/OrderDB,
-- OrderId trong PaymentDB la tham chieu giua cac service/Database.
-- Theo thiet ke microservices hien tai, cac cot nay KHONG co FOREIGN KEY
-- lien database; viec xac minh ton tai phai do Backend/API dam nhiem.
