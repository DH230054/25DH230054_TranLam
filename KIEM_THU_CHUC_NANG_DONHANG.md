# Kịch bản kiểm thử trên Visual Studio 2022 + SQL Server (local)

1. Chạy `CAPNHAT_DONHANG_1_LAN.cmd` (MyStore đã tồn tại) hoặc `CAIDAT_DATABASE_1_LAN.cmd` (MyStore mới). Kiểm tra trong SSMS `MyStore.dbo.[Order]` có `ShippingFee`.
2. Đăng nhập khách, thêm 2 sản phẩm, chọn Đặt hàng. Thử lần lượt Tiết kiệm/ Tiêu chuẩn/ Hỏa tốc, quan sát phí và tổng thay đổi theo lựa chọn. Đặt ít nhất 2 đơn với các hình thức khác nhau bằng COD.
3. Mở Đơn hàng của tôi: trang Tất cả đơn sẽ chia nhóm theo trạng thái; thử tìm theo mã đơn hoặc tên sản phẩm. Mua lại 1 sản phẩm để kiểm tra giỏ hàng tăng đúng 1. Chọn Xem chi tiết để mở trang sản phẩm.
4. Đăng nhập Admin, vào Đơn hàng. Thử lọc theo tên khách hàng, trạng thái thanh toán, ngày đặt; thử sắp xếp và phân trang 15 đơn mỗi trang; nhấn Chi tiết đơn hàng để thấy hình thức giao và phí.
5. Nhấn Cập nhật trạng thái để đổi đơn từ Chờ thanh toán → Đang xử lý → Đang vận chuyển → Đã giao, và cập nhật trạng thái thanh toán khi phù hợp. Quay về Lịch sử mua hàng khách để kiểm tra tab.
6. Tạo trên 2 đơn ở cùng trạng thái để kiểm tra nút Xem thêm; tạo trên 50 đơn nếu cần xác minh giới hạn 50.
7. Kiểm tra database cũ: không mất đơn, không đổi `TotalAmount` cũ; các đơn cũ có `ShippingFee=0`.

Chưa thể chạy kiểm thử tự động trong môi trường hiện tại vì không có Visual Studio/SQL Server Windows. Nếu gặp lỗi trong lớp, mở Error List hoặc Inner Exception và gửi ảnh để đối chiếu.
