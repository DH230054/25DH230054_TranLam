# 20 ẢNH SẢN PHẨM RIÊNG – DỰ ÁN BE_TRanMinhDong_25dh230010

Đã thay toàn bộ 20 ảnh nền có chữ MYSTORE bằng 20 ảnh **minh họa kiểu ảnh chụp** tương ứng với từng sản phẩm. Đây là ảnh tạo để minh họa bài tập, không phải ảnh thương mại thực tế của một hãng sản xuất. Mỗi sản phẩm có một JPG riêng nằm trong thư mục `MyStore.Web/Content/images/products/`.

## Đổi sang bản mới (khi máy đã có database MyStore)

1. Trong Visual Studio, nhấn Shift+F5 để dừng chạy và đóng project cũ.
2. Giải nén ZIP mới vào thư mục mới riêng (không ghi đè project cũ).
3. Mở `MyStore_MVC5_Full.sln` bằng Visual Studio 2022.
4. Chọn Build > Clean Solution, sau đó Build > Rebuild Solution, rồi nhấn F5.
5. Trong Chrome nhấn Ctrl+Shift+R hoặc Ctrl+F5 để tải lại ảnh.

**Không phải chạy lại SQL chỉ để thay ảnh.** Helper đã tự chọn ảnh cho 20 sản phẩm mẫu, kể cả khi cột ProductImage trong MyStore đang rỗng hoặc mang đường dẫn ảnh mẫu cũ. Đối với sản phẩm bạn thêm mới, có thể tiếp tục tải ảnh riêng ở trang quản trị Admin.

## Trường hợp cài trên máy trường

- Server mặc định: `(local)` với Windows Authentication (theo hình SSMS trường bạn gửi).
- Nếu máy trường chưa có database MyStore, chạy `CAIDAT_DATABASE_1_LAN.cmd` khi tài khoản có quyền tạo database.
- Các file SQL có sẵn trong thư mục `Database`. Nếu máy trường không có quyền chạy .cmd hoặc server khác tên, dùng `CHON_SERVER_SQL.cmd` hoặc mở script SQL trong SSMS để tạo database.
- Bản này vẫn giữ chức năng lựa chọn giao hàng và quản lý/lịch sử đơn hàng của bản mới nhất.

## Kiểm tra

Trên web, thử Sổ tay A5, Bàn phím cơ, Tai nghe Bluetooth, Áo thun basic và Ốp lưng điện thoại. Mỗi món phải có ảnh khác nhau, đúng loại hàng. Khi click vào sản phẩm, ảnh ở trang chi tiết phải trùng với ảnh trên danh sách.

Lưu ý: Chưa kiểm thử thực tế bằng Visual Studio/SQL Server Windows trong môi trường tạo file.
