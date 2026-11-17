-- MyStore MVC5 / EF6 - script tao/cap nhat DB da kiem tra cau truc.
-- Co the chay tren MyStore da co, KHONG XOA du lieu hien tai.
-- Chay bang CAIDAT_DATABASE_1_LAN.cmd (LocalDB) hoac VS SQL Server Object Explorer.
-- Script KHONG DROP DATABASE/TABLE va co the chay lai neu da co DB MyStore.
IF DB_ID(N'MyStore') IS NULL EXEC(N'CREATE DATABASE [MyStore]');
GO
USE MyStore;
GO
IF OBJECT_ID(N'dbo.[User]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[User] (
  Username NVARCHAR(255) NOT NULL PRIMARY KEY,
  [Password] NVARCHAR(256) NOT NULL,
  UserRole NCHAR(1) NOT NULL CHECK (UserRole IN ('A','C'))
);
END;
GO
IF OBJECT_ID(N'dbo.Customer', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Customer (
  CustomerID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  CustomerName NVARCHAR(120) NOT NULL,
  CustomerPhone NVARCHAR(15) NOT NULL,
  CustomerEmail NVARCHAR(MAX) NULL,
  CustomerAddress NVARCHAR(MAX) NULL,
  Username NVARCHAR(255) NOT NULL REFERENCES dbo.[User](Username)
);
END;
GO
IF OBJECT_ID(N'dbo.Category', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Category (
  CategoryID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  CategoryName NVARCHAR(120) NOT NULL
);
END;
GO
IF OBJECT_ID(N'dbo.Product', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Product (
  ProductID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  CategoryID INT NOT NULL REFERENCES dbo.Category(CategoryID),
  ProductName NVARCHAR(160) NOT NULL,
  ProductDecription NVARCHAR(MAX) NOT NULL, -- Tên cột typo theo Database2024.txt
  ProductPrice DECIMAL(18,2) NOT NULL,
  ProductImage NVARCHAR(MAX) NULL,
  StockQuantity INT NOT NULL CONSTRAINT DF_Product_Stock DEFAULT (20),
  CONSTRAINT CK_Product_Stock CHECK (StockQuantity>=0),
  CONSTRAINT CK_Product_Price CHECK (ProductPrice>=0)
);
END;
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[Order] (
  OrderID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  CustomerID INT NOT NULL REFERENCES dbo.Customer(CustomerID),
  OrderDate DATE NOT NULL,
  TotalAmount DECIMAL(18,2) NOT NULL,
  ShippingFee DECIMAL(18,2) NOT NULL CONSTRAINT DF_Order_ShippingFee DEFAULT (0),
  PaymentStatus NVARCHAR(30) NULL,
  ShippingAddress NVARCHAR(400) NOT NULL,
  DeliveryMethod NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Delivery DEFAULT ('Standard'),
  PaymentMethod NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Payment DEFAULT ('COD'),
  OrderStatus NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Status DEFAULT ('Pending')
);
END;
GO
IF OBJECT_ID(N'dbo.OrderDetail', N'U') IS NULL
BEGIN
CREATE TABLE dbo.OrderDetail (
  ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  ProductID INT NOT NULL REFERENCES dbo.Product(ProductID),
  OrderID INT NOT NULL REFERENCES dbo.[Order](OrderID),
  Quantity INT NOT NULL,
  UnitPrice DECIMAL(18,2) NOT NULL,
  TotalPrice AS (Quantity * UnitPrice) PERSISTED,
  CONSTRAINT CK_OrderDetail_Quantity CHECK (Quantity>0)
);
END;
GO
-- Idempotent updates: hỗ trợ bảng có sẵn được tạo từ Database2024.txt.
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.[User]') AND name = N'Password' AND max_length < 512)
  ALTER TABLE dbo.[User] ALTER COLUMN [Password] NVARCHAR(256) NOT NULL;
GO
IF OBJECT_ID(N'dbo.Product', N'U') IS NOT NULL AND COL_LENGTH('dbo.Product','StockQuantity') IS NULL
  ALTER TABLE dbo.Product ADD StockQuantity INT NOT NULL CONSTRAINT DF_Product_Stock_Upgrade DEFAULT (20) WITH VALUES;
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NOT NULL AND COL_LENGTH('dbo.[Order]','AddressDelivery') IS NOT NULL
   AND COL_LENGTH('dbo.[Order]','ShippingAddress') IS NULL
  EXEC sp_rename 'dbo.[Order].AddressDelivery','ShippingAddress','COLUMN';
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NOT NULL AND COL_LENGTH('dbo.[Order]','DeliveryMethod') IS NULL
  ALTER TABLE dbo.[Order] ADD DeliveryMethod NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Delivery_Upgrade DEFAULT ('Standard') WITH VALUES;
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NOT NULL AND COL_LENGTH('dbo.[Order]','PaymentMethod') IS NULL
  ALTER TABLE dbo.[Order] ADD PaymentMethod NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Payment_Upgrade DEFAULT ('COD') WITH VALUES;
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NOT NULL AND COL_LENGTH('dbo.[Order]','OrderStatus') IS NULL
  ALTER TABLE dbo.[Order] ADD OrderStatus NVARCHAR(30) NOT NULL CONSTRAINT DF_Order_Status_Upgrade DEFAULT ('Pending') WITH VALUES;
GO
IF OBJECT_ID(N'dbo.OrderDetail', N'U') IS NOT NULL AND COL_LENGTH('dbo.OrderDetail','TotalPrice') IS NULL
  ALTER TABLE dbo.OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
GO
-- Tài khoản đăng nhập đăng ký bằng website (PBKDF2). KHÔNG chèn mật khẩu plain text vào SQL.
-- Tạo admin đầu tiên: Đăng ký 1 tài khoản từ giao diện web, sau đó chạy:
-- UPDATE dbo.[User] SET UserRole='A' WHERE Username=N'TEN_DANG_NHAP_CUA_BAN';

-- Tuong thich voi database da tao truoc ban chon giao hang.
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NOT NULL AND COL_LENGTH('dbo.[Order]','ShippingFee') IS NULL
  ALTER TABLE dbo.[Order] ADD ShippingFee DECIMAL(18,2) NOT NULL CONSTRAINT DF_Order_ShippingFee_Upgrade DEFAULT (0) WITH VALUES;
GO
