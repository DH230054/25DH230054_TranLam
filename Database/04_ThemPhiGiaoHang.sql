-- CHAY 1 LAN (co the chay lai an toan) tren MyStore hien co.
-- GIU NGUYEN DON HANG CU, KHONG DROP/DELETE.
IF DB_ID(N'MyStore') IS NULL
BEGIN
    RAISERROR(N'Chua co database MyStore. Hay chay 01_Create_MyStore.sql truoc.', 16, 1);
    RETURN;
END;
GO
USE [MyStore];
GO
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NULL
BEGIN
    RAISERROR(N'Khong tim thay bang dbo.[Order]. Hay chay 01_Create_MyStore.sql truoc.', 16, 1);
    RETURN;
END;
GO
IF COL_LENGTH(N'dbo.[Order]', N'ShippingFee') IS NULL
    ALTER TABLE dbo.[Order] ADD ShippingFee DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Order_ShippingFee_New DEFAULT(0) WITH VALUES;
GO
-- Don hang cu duoc gan phi 0 de khong thay doi so tien da ghi nhan.
