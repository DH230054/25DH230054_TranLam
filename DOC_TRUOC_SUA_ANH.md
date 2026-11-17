# MyStore: sửa hiện ảnh 20 sản phẩm mẫu

**Nguyên nhân:** SQL Server vẫn có `ProductImage = NULL` vì được tạo trước khi có file ảnh.

**Đã sửa:** khi không có đường dẫn ảnh trong database, website tự sử dụng ảnh đúng tên của 20 sản phẩm có sẵn. Hiển thị ở trang chủ, danh sách, chi tiết, giỏ hàng và phần Admin. Không thay đổi đường dẫn ảnh của những sản phẩm bạn đã upload riêng.

## Cách chạy

1. Dừng Visual Studio bằng Shift+F5.
2. Giải nén ZIP mới vào thư mục khác thư mục cũ.
3. Mở `MyStore_MVC5_Full.sln` bằng Visual Studio 2022.
4. Build > Rebuild Solution; sau đó F5.
5. Trong trình duyệt nhấn Ctrl+F5.

Không cần chạy lại file SQL, không xóa database.

**Lưu ý:** Đây là 20 ảnh minh họa sản phẩm dạng card, chưa phải ảnh chụp sản phẩm ngoài đời.
