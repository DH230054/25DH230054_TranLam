# TranLam Shoes — hướng dẫn chạy

Đây là website bán giày được phát triển từ dự án ASP.NET MVC 5 trong file nguồn.

## Chức năng có sẵn
- Trang chủ, danh mục, tìm kiếm/lọc/sắp xếp sản phẩm và chi tiết sản phẩm.
- Đăng ký/đăng nhập, hồ sơ khách hàng.
- Giỏ hàng, checkout COD, lịch sử và trạng thái đơn hàng.
- Khu vực quản trị: quản lý sản phẩm, danh mục, khách hàng, đơn hàng và tài khoản quản trị.

## Chạy trên Windows
1. Cài Visual Studio 2022 và SQL Server.
2. Giải nén thư mục này.
3. Chạy `CAIDAT_DATABASE_1_LAN.cmd` để tạo database mẫu (máy trường dùng `(local)`).
4. Mở `25DH230054_TranLam.sln`, Restore NuGet, Rebuild Solution rồi nhấn F5.
5. Đăng nhập khu vực Admin để tải ảnh giày thật lên cho từng sản phẩm.

**Lưu ý:** Bộ ảnh trong file gốc là ảnh minh họa sản phẩm tổng hợp, không phải ảnh giày. Dữ liệu mẫu đã đổi thành danh mục và mặt hàng giày; hãy thay ảnh từng sản phẩm trong Admin trước khi đưa website vào sử dụng. Cần môi trường Windows/Visual Studio + SQL Server để build và chạy ASP.NET MVC 5; chưa xác nhận build trên máy hiện tại.
