> **Lưu ý bản dùng tại trường:** Mặc định SQL Server `(local)` với Windows Authentication. Hướng dẫn cập nhật nằm trong [`BAT_DAU_MAY_TRUONG.md`](BAT_DAU_MAY_TRUONG.md). Hướng dẫn LocalDB bên dưới là thông tin của bản cũ, không dùng cho máy trường.

# Hướng dẫn chạy MyStore trực tiếp bằng Visual Studio 2022

**Đây là dự án ASP.NET MVC5 trên .NET Framework 4.8**, KHÔNG phải ASP.NET Core / .NET 8. Bạn sẽ mở solution `.sln` có sẵn, không cần tạo project mới.

## 1. Cài phần mềm cần thiết trên máy Windows

- Visual Studio 2022 (Community hoặc bản khác). Trong Visual Studio Installer, chọn **ASP.NET and web development**. Nếu thiếu, mở **Modify** để bổ sung workload và **.NET Framework 4.8 targeting pack**. Có thể tham khảo file `.vsconfig` trong thư mục dự án.
- **SQL Server LocalDB**, đang có sẵn trên máy bạn theo ảnh `(localdb)\MSSQLLocalDB`. Không bắt buộc cài SSMS nếu dùng `CAIDAT_DATABASE_1_LAN.cmd`.
- Máy có Internet cho bước tải packages qua NuGet lần đầu.

## 2. Giải nén và mở solution

1. Giải nén toàn bộ file ZIP ra thư mục đơn giản, ví dụ `D:\MyStore_VS2022` (không mở `.sln` nằm trực tiếp bên trong ZIP).
2. Mở `MyStore_MVC5_Full.sln` bằng Visual Studio 2022, hoặc nhấp đúp `MO_BANG_VISUAL_STUDIO.cmd`.
3. Trong Solution Explorer, bảo đảm nhìn thấy project **MyStore.Web**. Chuột phải **MyStore.Web** → **Set as Startup Project**.
4. Chọn **Build → Restore NuGet Packages** hoặc chuột phải Solution → **Restore NuGet Packages**. Nếu không thấy, chọn **Build → Rebuild Solution** hoặc mở **Tools → NuGet Package Manager → Package Manager Console** và nhập: `Update-Package -Reinstall` (chỉ dùng khi restore bị lỗi).

> Nếu xuất hiện thông báo thiếu ASP.NET project support, hãy mở Visual Studio Installer → Modify → **ASP.NET and web development**; đóng Visual Studio rồi mở lại.

## 3. Tạo database MyStore (đã cấu hình sẵn LocalDB)

**Cách nhanh nhất trên máy của bạn:** chạy `CAIDAT_DATABASE_1_LAN.cmd` sau khi giải nén. File này tự tạo/cập nhật bảng và thêm dữ liệu mẫu, không cần nhập 2 file riêng. Nếu muốn chạy thủ công, dùng một file `Database/00_CHAY_1_FILE_SQL.sql`.

Các bước dùng SSMS dưới đây chỉ là cách thay thế.

1. Nếu cài SQL thủ công, mở **SQL Server Object Explorer** trong Visual Studio và dùng đúng tên server của bạn là `(localdb)\MSSQLLocalDB`. Chọn **Windows Authentication**. Có thể dùng SSMS nếu đã cài.
2. Mở `Database\00_CHAY_1_FILE_SQL.sql`, copy toàn bộ và dán vào cửa sổ New Query trên cùng server LocalDB. Nhấn **Execute** để tạo bảng và thêm dữ liệu mẫu trong một lần.
3. Không cần chạy thêm file dữ liệu mẫu nếu đã chạy file `00_CHAY_1_FILE_SQL.sql`.
4. Kiểm tra bên trái trong **Databases → MyStore → Tables**. Nếu chưa thấy, chuột phải `Databases` → **Refresh**.

Nếu bạn đã chạy SQL database gốc trước đó, script `01_Create_MyStore.sql` cũng có các câu lệnh thêm cột cho bài tập. Nên **sao lưu database trước** nếu dùng dữ liệu thật.

## 4. Sửa kết nối database trong Visual Studio

Trong Solution Explorer, mở `MyStore.Web` → `Web.config`, kiểm tra:

```xml
<add name="MyStoreEntities"
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MyStore;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

- Nếu SSMS kết nối bằng `localhost`, giữ nguyên.
- Nếu SSMS kết nối bằng `.\SQLEXPRESS`, sửa thành `Data Source=.\SQLEXPRESS;`.
- Với bản này, `(localdb)\MSSQLLocalDB` **đã được cấu hình sẵn**, không phải sửa `Web.config` nữa.
- Nếu server tên khác, thay `Data Source=` bằng *chính xác Server name đang dùng trong SSMS*.

Lưu Ctrl+S. Đừng đăng công khai mật khẩu SQL Server hoặc khóa thanh toán.

## 5. Chạy website trong Visual Studio

1. Trên thanh công cụ, chọn **Debug**, **Any CPU**, và nút chạy **IIS Express**.
2. Chọn **Build → Rebuild Solution**. Trong cửa sổ **Error List**, kiểm tra **Errors = 0** trước khi chạy.
3. Nhấn **F5** để chạy debug, hoặc **Ctrl+F5** để chạy không debug. Trình duyệt mở website local (thường `http://localhost:50551/` hoặc cổng khác Visual Studio tự gán).
4. Thử `Đăng ký` → `Đăng nhập` → `Sản phẩm` → `Giỏ hàng` → `Đặt hàng` (chọn COD).
5. Muốn vào Admin: đăng ký một tài khoản mới bằng giao diện web, rồi quay lại SSMS chạy (thay username thật):

```sql
USE MyStore;
UPDATE dbo.[User] SET UserRole=N'A' WHERE Username=N'tendangnhapcua_ban';
```

Đăng xuất/đăng nhập lại, mở đường dẫn `/Admin`.

## 6. Lỗi thường gặp và cách sửa

| Thông báo | Cách xử lý |
|---|---|
| Project is incompatible / ASP.NET web application support not installed | Visual Studio Installer → Modify → **ASP.NET and web development**. |
| MSB3644 / missing reference assemblies for .NET Framework v4.8 | Cài **.NET Framework 4.8 Targeting Pack** từ Visual Studio Installer. |
| The type or namespace name 'Mvc'/'EntityFramework' does not exist / missing assembly | Kết nối Internet; chuột phải Solution → **Restore NuGet Packages**, rồi **Rebuild**. Kiểm tra thư mục `packages` xuất hiện trong thư mục solution. |
| Cannot open database `MyStore` requested by login | Chạy hai file SQL trên đúng SQL Server và sửa **Data Source** trong `Web.config`. Kiểm tra quyền Windows Authentication của IIS Express. |
| A network-related or instance-specific error occurred / error: 26 or 40 | Kiểm tra SSMS kết nối được instance, SQL Server đang chạy, tên `Data Source` có chính xác không. |
| HTTP Error 500.19 (web.config) | Kiểm tra IIS Express và workload ASP.NET; không dùng `dotnet run` hoặc Kestrel vì đây là ứng dụng .NET Framework MVC5. |
| HTTP 404 khi mở `/Admin` | Đăng nhập bằng tài khoản đã đổi `UserRole` thành `A`, và truy cập đúng `http://localhost:<port>/Admin`. |
| Tiếng Việt bị lỗi font | Lưu file nguồn bằng UTF-8, dùng **File → Save As → Save with Encoding → UTF-8**, kiểm tra SQL dùng tiền tố `N'...'` cho chuỗi tiếng Việt. |
| Gặp lỗi biên dịch khác | Chụp cửa sổ **Error List** sau Rebuild, gửi ảnh hoặc copy những dòng lỗi đầu tiên để xác định chính xác file và dòng cần sửa. |

## Lưu ý về mức độ xác minh

Bộ nguồn đã qua kiểm tra tĩnh (cấu trúc solution, project XML, tài liệu, đường dẫn và file nguồn). **Chưa build thành công trực tiếp trong Visual Studio và chưa chạy với SQL Server trên Windows trong môi trường tạo file này**, vì không có các ứng dụng đó; kết quả cuối cùng cần kiểm tra thực tế trên máy bạn. Nếu có lỗi, ghi lại thông báo **Error List** để xử lý đúng nguyên nhân.
