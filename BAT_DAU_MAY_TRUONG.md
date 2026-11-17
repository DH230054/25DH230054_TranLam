# BE_TranMinhDong_25dh230010 — bản dùng trên máy trường

**Server theo ảnh ở trường:** `(local)` · **Windows Authentication**.

## Chạy nhanh trên máy trường

1. Giải nén ZIP vào thư mục thông thường (không mở code trực tiếp trong ZIP).
2. Kiểm tra SQL Server tên `(local)` có **Connect** thành công trong SSMS. Kết nối thử bằng Windows Authentication.
3. Chạy `CAIDAT_DATABASE_1_LAN.cmd` (file loại *Windows Command Script*, không phải `.ps1`). Đợi `SUCCESS - MyStore database ready`.
   - File này tạo/cập nhật bảng, nạp 20 dữ liệu mẫu nếu bảng Product còn trống, và cập nhật đường dẫn ảnh; **không xóa dữ liệu đang có**.
   - Công cụ cũng đồng bộ `MyStore.Web/Web.config` thành `Data Source=(local)`.
4. Mở `MyStore_MVC5_Full.sln` bằng **Visual Studio 2022**; Restore NuGet Packages nếu được yêu cầu.
5. Build → Rebuild Solution → F5 bằng IIS Express.
6. Ở trình duyệt, Ctrl+F5 để cập nhật cache. Các ảnh được lưu trong `MyStore.Web/Content/images/products`.

## Dùng trên máy cá nhân hoặc server khác

Chạy `CHON_SERVER_SQL.cmd` và chọn tên SQL Server đúng với máy đó. Công cụ sẽ cùng lúc **tạo/cập nhật MyStore** và **sửa chuỗi kết nối trong Web.config**. Hỗ trợ LocalDB, Express và tên khác; dùng Windows Authentication.

### Không có quyền tạo database ở trường?

Cần tài khoản có quyền `CREATE DATABASE`, hoặc nhờ giáo viên/phòng máy cấp quyền. Nếu được phép tạo DB thủ công, trong SSMS đang Connect đến server `(local)`, chạy theo thứ tự:

- `Database/01_Create_MyStore.sql`
- `Database/02_Seed_SampleData.sql`
- `Database/03_CapNhat_Anh_SanPham.sql`

Sau đó giữ `MyStore.Web/Web.config` mặc định `(local)` và mở Visual Studio 2022. Chỉ chạy trên server trường khi được phép. Không chia sẻ mật khẩu SQL.

### Thông tin quan trọng

- Mục **Server name** trong ô đăng nhập SQL Server chưa chứng minh chắc chắn là đã kết nối; cần bấm **Connect** và thấy database trong Object Explorer.
- Bộ ảnh gồm **20 hình minh họa**, không phải ảnh chụp sản phẩm thật. Nếu thêm sản phẩm mới, upload ảnh trong Admin.
- Nếu trang trắng hoặc báo `SqlException`, kiểm tra thông báo lỗi, đúng SQL instance, quyền đăng nhập và service SQL Server.
- Trường hợp đã có database MyStore ở trường, script không xóa dữ liệu cũ, chỉ tạo phần còn thiếu và cập nhật ảnh của sản phẩm mẫu cùng tên.
- Dự án chưa được build/run trực tiếp trên máy Windows trong môi trường tạo ZIP; hãy thử Rebuild + F5 để xác nhận thực tế.
