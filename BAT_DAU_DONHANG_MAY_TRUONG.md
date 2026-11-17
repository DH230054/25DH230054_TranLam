# Hướng dẫn cập nhật tính năng đơn hàng (máy trường SQL Server `(local)`)

Đây là **bản tiếp nối** của `BE_TranMinhDong_25dh230010`, sử dụng Visual Studio 2022, ASP.NET MVC5, Entity Framework 6, .NET Framework 4.8 và SQL Server `(local)` (Windows Authentication).

## Sử dụng ngay trên database MyStore đã có (không xóa dữ liệu)

1. Dừng IIS Express trong Visual Studio bằng **Shift+F5**, giải nén ZIP mới vào thư mục mới.
2. Trong thư mục `MyStore_MVC5_Full`, nhấp đôi **`CAPNHAT_DONHANG_1_LAN.cmd`**. Nó chỉ thêm cột `ShippingFee` nếu chưa có, không xóa đơn cũ.
3. Chờ thông báo `SUCCESS - ShippingFee ready on (local)`; nếu bị từ chối quyền thì nhờ người quản trị máy trường cho phép thay đổi database, hoặc mở `Database/04_ThemPhiGiaoHang.sql` bằng SSMS có đủ quyền và Execute.
4. Nhấp đôi **`MyStore_MVC5_Full.sln`**, khôi phục NuGet nếu được yêu cầu, chọn **Build -> Rebuild Solution**, sau đó **F5**.
5. Đăng nhập tài khoản khách, thêm hàng vào giỏ, đến **Đặt hàng**, chọn 1 trong 3 hình thức giao rồi xác nhận. Trên trang **Đơn hàng của tôi** xem tab trạng thái/tìm kiếm/mua lại. Đăng nhập Admin để thử tìm kiếm và cập nhật đơn hàng.

Nếu máy là database mới, có thể chạy `CAIDAT_DATABASE_1_LAN.cmd`: chương trình tạo/kiểm tra bảng, thêm sản phẩm mẫu + ảnh và thêm cột phí giao hàng. **Không chạy file `.ps1` bằng cách nhấp đúp**; chạy `.cmd` tương ứng.

## Phí giao hàng minh họa (chưa được PowerPoint chỉ định)

| Phương thức | Phí mẫu |
|---|---:|
| Giao hàng tiết kiệm | 15.000₫ |
| Giao hàng tiêu chuẩn | 30.000₫, miễn phí khi tiền hàng từ 500.000₫ |
| Giao hàng hỏa tốc | 50.000₫ |

Đổi giá trong `MyStore.Web/Services/ShippingOptions.cs`. Server luôn tự tính phí từ giỏ hàng, không nhận phí người dùng tự gửi lên. Đơn cũ giữ nguyên tổng tiền cũ với `ShippingFee=0`.

## Các chức năng theo Ktra BE.pptx

- Admin: danh sách đơn, lọc tên khách/trạng thái thanh toán/ngày đặt, sắp xếp, 15 đơn/trang; `DetailedOrder` hiển thị danh sách sản phẩm, giá, phí giao hàng; `UpdateStatus` cập nhật trạng thái và thanh toán; dashboard thống kê đơn.
- Customer: Lịch sử mua hàng (`MyOrder`) tìm mã đơn hoặc tên sản phẩm, năm trạng thái, 2 đơn mới nhất/nhóm, nút Xem thêm tối đa 50, nút Mua lại thêm 1 sản phẩm vào giỏ, link Xem chi tiết sản phẩm, và link xem chi tiết đơn.
- Vận chuyển: 3 lựa chọn tiết kiệm/tiêu chuẩn/hỏa tốc được lưu trong đơn hàng, hiển thị ở Admin, xác nhận đơn và lịch sử mua hàng; tổng tiền cộng phí giao hàng.
- PayPal chỉ chạy khi có cấu hình Sandbox thật, **không tự giả lập thanh toán**.

## Kiểm thử và giới hạn

Kiểm tra lại trên máy trường vì môi trường tạo file không cài Visual Studio hay SQL Server. Không thể cam kết đã build và chạy toàn bộ luồng trên Windows. SQL Server trường phải cho phép tài khoản Windows đọc/ghi database và có quyền cập nhật schema `dbo.[Order]` ở lần nâng cấp.
