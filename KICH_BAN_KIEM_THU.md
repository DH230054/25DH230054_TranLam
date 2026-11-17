# Kịch bản kiểm thử thủ công trong Visual Studio

| Test | Bước thực hiện | Kết quả mong đợi |
|---|---|---|
| 1 | Đăng ký tài khoản A rồi đăng nhập | Đăng ký thành công, không lỗi tiếng Việt |
| 2 | Đăng ký trùng username A | Hiện lỗi username đã tồn tại |
| 3 | Đăng ký mật khẩu dưới 8 ký tự, số điện thoại sai, email sai | ModelState / ValidationMessage từ chối |
| 4 | Tìm sản phẩm theo từ khóa, giá 100.000–300.000, sắp xếp tăng rồi giảm | Chỉ hiện sản phẩm hợp lệ, phân trang hoạt động |
| 5 | Truy cập `/Admin` với user thường | Từ chối 403 |
| 6 | Nâng quyền A lên Admin bằng SQL; đăng nhập lại | Mở được `/Admin` |
| 7 | Thêm / sửa / xóa danh mục không có sản phẩm | CRUD chạy, hiển thị tiếng Việt |
| 8 | Xóa danh mục đang có sản phẩm | Hệ thống chặn xóa |
| 9 | Upload ảnh JPG, thử gửi exe đổi đuôi .jpg | Ảnh JPG thành công, file giả mạo bị từ chối |
| 10 | Thêm sản phẩm, sửa giá, sửa tồn kho, lọc và phân trang | Dữ liệu cập nhật từ SQL |
| 11 | Thêm 2 sản phẩm vào giỏ, thay số lượng từ 1 → 3 | Tiền tạm tính tăng đúng; xóa/giảm số lượng tiền giảm |
| 12 | Bỏ tick 1 sản phẩm | Tổng tiền chỉ tính sản phẩm đã chọn |
| 13 | Đang có giỏ hàng → Đăng nhập → Đăng xuất → Đăng nhập | Các món vẫn còn trong giỏ Session |
| 14 | Đặt đơn COD hợp lệ | Tạo Order, OrderDetail, trừ tồn kho theo quantity, clear các món đã mua |
| 15 | Đặt quá tồn kho hoặc sửa giỏ và giá sản phẩm trong DB | Hệ thống kiểm tra lại và không xuất kho âm |
| 16 | Vào MyOrder → Hủy đơn đang Pending | Đơn Cancelled, tồn kho được cộng trở lại, doanh thu Dashboard giảm tương ứng |
| 17 | Nhấn hủy 2 lần hoặc hủy đơn đang Shipping | Không cộng tồn kho lặp lại, thông báo không hủy được |
| 18 | Admin tạo user Admin mới và thay mật khẩu | Quyền/đăng nhập mới có hiệu lực |
| 19 | Người khác truy cập chi tiết đơn của user A bằng URL | 404; không xem được dữ liệu người khác |
| 20 | Cấu hình PayPal Sandbox và thử thanh toán | Redirect sang PayPal Sandbox, capture xác nhận và lưu Order; cần thiết lập ngoài |

**Chú ý:** Danh sách trên là kịch bản để người dùng chạy trên Windows, **không phải bằng chứng các test đã chạy trong môi trường tạo file**.
