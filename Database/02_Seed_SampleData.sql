-- Chay sau 01_Create_MyStore.sql. Co the chay lai, khong tu dong xoa du lieu.
USE MyStore;
GO
-- Danh muc cua cua hang giay TranLam Shoes.
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName=N'Giày Sneaker')
  INSERT INTO dbo.Category (CategoryName) VALUES (N'Giày Sneaker');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName=N'Giày Thể Thao')
  INSERT INTO dbo.Category (CategoryName) VALUES (N'Giày Thể Thao');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName=N'Giày Công Sở')
  INSERT INTO dbo.Category (CategoryName) VALUES (N'Giày Công Sở');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName=N'Dép & Sandal')
  INSERT INTO dbo.Category (CategoryName) VALUES (N'Dép & Sandal');
IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName=N'Phụ Kiện Giày')
  INSERT INTO dbo.Category (CategoryName) VALUES (N'Phụ Kiện Giày');
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Product)
BEGIN
 DECLARE @sneaker INT=(SELECT TOP 1 CategoryID FROM dbo.Category WHERE CategoryName=N'Giày Sneaker');
 DECLARE @sport INT=(SELECT TOP 1 CategoryID FROM dbo.Category WHERE CategoryName=N'Giày Thể Thao');
 DECLARE @office INT=(SELECT TOP 1 CategoryID FROM dbo.Category WHERE CategoryName=N'Giày Công Sở');
 DECLARE @sandal INT=(SELECT TOP 1 CategoryID FROM dbo.Category WHERE CategoryName=N'Dép & Sandal');
 DECLARE @accessory INT=(SELECT TOP 1 CategoryID FROM dbo.Category WHERE CategoryName=N'Phụ Kiện Giày');
 INSERT INTO dbo.Product(CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage,StockQuantity) VALUES
 (@sneaker,N'Sneaker Urban Classic',N'Sneaker phong cách tối giản, dễ phối đồ hằng ngày.',699000,N'~/Content/images/products/ao-thun-basic.jpg',30),
 (@sneaker,N'Sneaker Street Low',N'Phom low-top trẻ trung, đế cao su bám đường.',829000,N'~/Content/images/products/ao-khoac-nhe.jpg',24),
 (@sneaker,N'Sneaker Retro 90s',N'Cảm hứng retro năng động với phối màu nổi bật.',959000,N'~/Content/images/products/tui-tote-canvas.jpg',18),
 (@sneaker,N'Sneaker Mono White',N'Tông trắng thanh lịch, phù hợp nhiều phong cách.',749000,N'~/Content/images/products/vi-dung-the.jpg',28),
 (@sport,N'Giày chạy bộ AirFlex',N'Thiết kế nhẹ, đệm êm hỗ trợ chạy bộ mỗi ngày.',1199000,N'~/Content/images/products/chuot-khong-day.jpg',20),
 (@sport,N'Giày Training Pro',N'Đế ổn định, phù hợp tập gym và vận động đa năng.',1099000,N'~/Content/images/products/ban-phim-co-compact.jpg',16),
 (@sport,N'Giày bóng rổ High Court',N'Cổ cao ôm chân, hỗ trợ di chuyển trên sân.',1499000,N'~/Content/images/products/tai-nghe-bluetooth.jpg',12),
 (@sport,N'Giày đi bộ CloudStep',N'Lót giày mềm mại, thoải mái khi đi bộ lâu.',899000,N'~/Content/images/products/den-ban-led-usb.jpg',22),
 (@office,N'Giày Oxford Classic',N'Kiểu dáng Oxford lịch lãm dành cho môi trường công sở.',1250000,N'~/Content/images/products/but-may-toi-gian.jpg',14),
 (@office,N'Giày Derby Daily',N'Derby hiện đại, dễ kết hợp cùng trang phục smart-casual.',1150000,N'~/Content/images/products/so-tay-a5.jpg',17),
 (@office,N'Giày Loafer Premium',N'Loafer tiện lợi với thiết kế tinh tế, thanh lịch.',1090000,N'~/Content/images/products/so-ke-hoach-tuan.jpg',15),
 (@office,N'Giày Tây Basic',N'Mẫu giày tây cơ bản cho đi làm và sự kiện.',990000,N'~/Content/images/products/ly-su-dang-tron.jpg',19),
 (@sandal,N'Sandal quai ngang',N'Sandal nhẹ nhàng, thoáng chân cho ngày năng động.',399000,N'~/Content/images/products/binh-nuoc-giu-nhiet.jpg',32),
 (@sandal,N'Dép quai chữ H',N'Dép tiện dụng với kiểu dáng tối giản.',329000,N'~/Content/images/products/op-lung-dien-thoai.jpg',26),
 (@sandal,N'Sandal trekking',N'Quai chắc chắn, đế bám tốt cho hoạt động ngoài trời.',599000,N'~/Content/images/products/gia-do-dien-thoai.jpg',18),
 (@sandal,N'Dép slides comfort',N'Lòng dép êm ái, phù hợp sử dụng hằng ngày.',289000,N'~/Content/images/products/hop-luu-tru-mini.jpg',35),
 (@accessory,N'Bộ dây giày Classic',N'Dây giày thay thế bền chắc, nhiều cách phối.',59000,N'~/Content/images/products/day-deo-the.jpg',60),
 (@accessory,N'Bộ vệ sinh giày',N'Làm sạch và chăm sóc giày sau mỗi lần sử dụng.',129000,N'~/Content/images/products/khay-dung-do-go.jpg',40),
 (@accessory,N'Lót giày Comfort',N'Lót giày mềm mại, hỗ trợ nâng đỡ bàn chân.',99000,N'~/Content/images/products/but-gel-6-mau.jpg',45),
 (@accessory,N'Túi đựng giày du lịch',N'Túi gọn nhẹ giúp bảo quản giày khi di chuyển.',159000,N'~/Content/images/products/mu-luoi-trai.jpg',25);
END;
GO
