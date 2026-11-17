> **Lưu ý bản dùng tại trường:** Mặc định SQL Server `(local)` với Windows Authentication. Hướng dẫn cập nhật nằm trong [`BAT_DAU_MAY_TRUONG.md`](BAT_DAU_MAY_TRUONG.md). Hướng dẫn LocalDB bên dưới là thông tin của bản cũ, không dùng cho máy trường.

# MyStore MVC5 — Dự án website bán hàng

**Công nghệ:** ASP.NET MVC 5 (.NET Framework 4.8), C#, Entity Framework 6.4.4, SQL Server, Razor, HTML/CSS/JavaScript. Mã nguồn **UTF-8** để hiển thị tiếng Việt.

## CÁCH MỞ VÀ CHẠY (ĐỌC KỸ)

**Trước tiên, chạy `CAIDAT_DATABASE_1_LAN.cmd` để tạo database tự động trên `(localdb)\MSSQLLocalDB`. Sau đó mở `MyStore_MVC5_Full.sln` bằng Visual Studio 2022**, không tạo project ASP.NET mới. Có thể chạy `MO_BANG_VISUAL_STUDIO.cmd` để mở nhanh solution. Tài liệu riêng `HUONG_DAN_VISUAL_STUDIO.md` có các bước và cách xử lý lỗi hay gặp.

1. Trên Windows, cài **Visual Studio 2022**, chọn workload **ASP.NET and web development**, đảm bảo đã có **.NET Framework 4.8 Targeting Pack**, **SQL Server LocalDB**. Máy bạn đã thấy `(localdb)\MSSQLLocalDB` trong SQL Server Object Explorer; không cần cài thêm SSMS nếu chạy file `.cmd`.
2. **Máy bạn dùng `(localdb)\MSSQLLocalDB`**, đã cấu hình sẵn `Web.config`: chạy `CAIDAT_DATABASE_1_LAN.cmd` để tự tạo/cập nhật MyStore và thêm dữ liệu mẫu; KHÔNG xóa database cũ.
3. Nếu không dùng `.cmd`, mở `Database/00_CHAY_1_FILE_SQL.sql` từ SSMS/SQL Server Object Explorer và bấm **Execute** để chạy 1 lần.
4. Kiểm tra `MyStore → Tables` trong SQL Server Object Explorer, refresh nếu cần.
5. Mở **`MyStore_MVC5_Full.sln`** bằng Visual Studio 2022. Chọn chuột phải Solution → **Restore NuGet Packages**. Đợi tải EntityFramework, MVC5, Razor, Newtonsoft.Json.
6. **`MyStore.Web/Web.config` đã được sửa sẵn để dùng `(localdb)\MSSQLLocalDB`** đúng ảnh SQL Server bạn gửi. Không cần sửa kết nối.
7. Chọn chuột phải project **MyStore.Web → Set as Startup Project**, chọn **Build → Rebuild Solution**, sau đó **F5** (IIS Express). Nếu Visual Studio báo phải enable SSL hoặc chọn server, cứ dùng cấu hình localhost phù hợp để phát triển.
8. Tại website, chọn **Đăng ký** và tạo một tài khoản (mật khẩu tối thiểu 8 ký tự). Để cấp quyền **Admin lần đầu**, dùng SQL Server Object Explorer → New Query và chạy:  
   `USE MyStore; UPDATE dbo.[User] SET UserRole=N'A' WHERE Username=N'TEN_DANG_NHAP_CUA_BAN';`  
   *Đổi đúng `TEN_DANG_NHAP_CUA_BAN` thành tài khoản vừa đăng ký.* Sau đó đăng xuất, đăng nhập lại và mở `/Admin`.
9. Quản trị viên có thể tạo thêm admin trong mục **Tài khoản Admin**.

## ĐỊA CHỈ TRANG

- Trang khách hàng: `/` hoặc `/Home/Index`
- Danh sách / tìm kiếm sản phẩm: `/Home/ProductList`
- Giỏ hàng: `/Cart/Index`
- Đăng ký: `/Account/Register`
- Đăng nhập: `/Account/Login`
- Thanh toán: `/Order/Checkout`
- Đơn hàng của tôi: `/Order/MyOrder`
- Quản trị: `/Admin`
- Quản lý danh mục: `/Admin/Categories`
- Quản lý sản phẩm: `/Admin/Products`
- Khách hàng: `/Admin/Customers`
- Đơn hàng: `/Admin/Orders`
- Tài khoản quản trị: `/Admin/Users`

## CẤU TRÚC THƯ MỤC

- `MyStore.Web/Models/Entities.cs`: các thực thể tương ứng bảng SQL.
- `MyStore.Web/Models/MyStoreEntities.cs`: EF6 DbContext ánh xạ cơ sở dữ liệu có sẵn.
- `MyStore.Web/Models/ViewModels.cs`: model cho form và tìm kiếm; Data Annotation.
- `MyStore.Web/Services`: mật khẩu PBKDF2, quyền admin, giỏ session, đặt/hủy đơn, PayPal REST Sandbox.
- `MyStore.Web/Controllers`: các chức năng dành cho khách hàng.
- `MyStore.Web/Views`: Razor page, partial view, layout khách hàng.
- `MyStore.Web/Areas/Admin`: Controller, partial view, layout và các trang quản trị.
- `MyStore.Web/Content`: CSS và thư mục ảnh upload khi chạy.
- `Database`: SQL tạo bảng, bổ sung cột và dữ liệu mẫu; có bản SQL gốc tham khảo.

## GIẢI THÍCH HAI ĐIỂM KHÁC VỚI VIDEO

1. **EF6 Database First:** Dự án giữ đúng *SQL Server database được tạo trước* và ánh xạ lớp dữ liệu bằng **DbContext + Data Annotations/Fluent API**, **không sử dụng EDMX Designer**. Cách này vẫn đọc/ghi các bảng gốc, tránh phụ thuộc file EDMX tự sinh theo tên máy và server SQL của người quay video. Nếu giảng viên bắt buộc phải có **`.edmx`**, bạn cần tạo EDMX bằng Visual Studio → **Models → Add → New Item → ADO.NET Entity Data Model → EF Designer from database** sau khi kết nối SQL, rồi điều chỉnh project theo thực thể được sinh; không thể đồng thời dùng hai lớp context trùng model trong cùng source mà không sửa.
2. **Phân trang:** Có phân trang server-side với `Skip/Take` và Razor (`_Pager`, `_PagerAdmin`), thay vì cài thêm `PagedList.Mvc`. Kết quả chức năng tương đương nhưng không phải cùng một thư viện với video.

## PAYPAL (TUỲ CHỌN TRONG PPT)

- COD (thanh toán khi nhận hàng) chạy độc lập, không cần khóa API.
- Trong `Web.config`, cấu hình `PayPalClientId` và `PayPalClientSecret` của tài khoản **PayPal Sandbox** để thử thanh toán PayPal. `VndPerUSD` là tỷ giá demo có thể chỉnh trong cấu hình. PayPal REST Orders v2 sử dụng **USD**, không phải chuyển tiền VND trực tiếp.
- Đây là **demo học tập**, chưa sẵn sàng chạy PayPal live: cần webhook, database ledger giữ giao dịch, xử lý idempotency, tự động hoàn tiền nếu capture thành công nhưng tạo đơn thất bại, kiểm tra tỷ giá và đối soát.
- Khi chưa có khóa Sandbox, chọn COD hoặc sẽ nhận thông báo chưa cấu hình. Không cung cấp thông tin PayPal thật cho ChatGPT.

## LƯU Ý AN TOÀN VÀ KHẢ NĂNG KIỂM THỬ

- Cần chạy **Windows/Visual Studio/.NET Framework 4.8/IIS Express/SQL Server**. Môi trường tạo mã nguồn không có các công cụ đó, vì vậy **chưa thể build và kiểm thử web end-to-end trong Visual Studio**; cần thực hiện bước 7 và xử lý lỗi phát sinh tại máy bạn nếu có.
- Demo dùng FormsAuthentication và CSRF token cho các POST, password hashing PBKDF2, phân quyền admin, kiểm tra tồn kho khi đặt hàng. Cần bật HTTPS, thay `customErrors=Off`, bảo vệ secrets, bổ sung rate limiting, logging, xác minh email, kiểm thử bảo mật trước khi xuất bản lên Internet.
- Các hình sản phẩm mẫu hiển thị hình placeholder nếu không upload; muốn có ảnh thật vào **Admin → Sản phẩm → Sửa → Tải ảnh lên**.
- File SQL gốc có cột `ProductDecription` viết sai chính tả; code ánh xạ tên property `ProductDescription` vào cột này để tránh lỗi lệch database. Cột `AddressDelivery` gốc được đổi tên sang `ShippingAddress` theo PPT phần Customer.
- Nếu có tài khoản trong SQL cũ lưu mật khẩu dạng văn bản thuần, sẽ không đăng nhập được qua hệ thống PBKDF2 mới; phải đặt lại mật khẩu hợp lệ, không tự động chuyển nguyên văn.
