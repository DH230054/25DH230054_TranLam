# Đối chiếu PowerPoint và yêu cầu viết trên bảng

Nguồn: `Web BE Development_p1 Project Setup`, `Web BE Development_p2 Admin site`, `Web BE Development_p3 Customer Site`, `database2024.txt` và các video BE2024 gửi kèm.

## Phần 1 — Project Setup

- [x] SQL Server: tạo MyStore, 6 bảng chính (User, Customer, Category, Product, Order, OrderDetail), quan hệ khóa ngoại.
- [x] ASP.NET MVC5 / EF6 / database-first mapping.
- [x] Phân vùng Area `Admin` với controller và layout riêng.
- [x] Khắc phục trùng HomeController qua namespaces ở RouteConfig và AdminAreaRegistration.
- [x] Tạo script SQL bổ sung và cập nhật model khi schema phát sinh thêm các cột.
- [~] EDMX Designer: hướng dẫn tạo bổ sung bằng Visual Studio; dự án đang dùng DbContext ánh xạ trực tiếp thay cho `.edmx` (xem README).

## Phần 2 — Admin Site

- [x] `Areas/Admin/Views/Shared/_AdminLayout.cshtml` có header/footer, menu.
- [x] Category CRUD: Index, Details, Create, Edit, Delete; PartialView `_PVCreateEditCategory`.
- [x] Product CRUD: Index, Details, Create, Edit, Delete; partial forms `_PVCreateEditProduct`, `_PVProductInfo`.
- [x] Upload file ảnh JPG/PNG/WEBP lên `Content/images`, hình default nếu chưa có.
- [x] Data Annotation/validation, giao diện thân thiện tiếng Việt.
- [x] Tìm kiếm tên/mô tả/danh mục; lọc khoảng giá; sắp xếp theo tên và giá; phân trang.
- [x] Xem/tìm kiếm/sắp xếp/phân trang khách hàng; xem lịch sử đơn của một khách.
- [x] Quản trị đơn: tìm kiếm, lọc trạng thái, xem chi tiết, cập nhật tiến trình, hủy đơn đang chờ.
- [x] Quản lý tài khoản Admin: tạo, đổi mật khẩu, xóa, tìm kiếm, phân trang; phân quyền bằng AdminOnly.
- [x] Tổng quan Admin với số liệu tổng hợp và biểu đồ thanh đơn giản theo tháng (không sử dụng Google Chart CDN).

## Phần 3 — Customer Site

- [x] `_CustomerLayout`, menu danh mục `_CategoriesPV`, menu tài khoản `_PersonalMenuPV`.
- [x] Trang Home: sản phẩm nổi bật, mới, danh mục.
- [x] Tìm kiếm, lọc sản phẩm theo danh mục, giá; sắp xếp và phân trang.
- [x] Chi tiết sản phẩm và sản phẩm tương tự.
- [x] Đăng ký, đăng nhập, đăng xuất, cập nhật hồ sơ, đổi mật khẩu.
- [x] Giỏ hàng lưu Session: thêm/xóa/cập nhật số lượng, chọn sản phẩm để mua, nhóm theo danh mục và partial sản phẩm tương tự.
- [x] Trang Checkout: địa chỉ giao hàng, phương thức giao/ thanh toán, COD.
- [x] Xác nhận đơn hàng: mã, sản phẩm, số lượng, thành tiền, trạng thái.
- [x] Trang MyOrder: lọc theo trạng thái, tìm theo mã đơn/tên sản phẩm, xem chi tiết, hủy đơn.
- [~] PayPal: đã tích hợp luồng REST Sandbox khi nhập khóa; chưa thể thử thanh toán vì chưa có tài khoản Sandbox/kết nối máy chạy.

## Yêu cầu trên ảnh bảng bổ sung (làm sau phần PPT/video)

- [x] **Ràng buộc dữ liệu (CRUD):** [Required], [StringLength], [Range], kiểm tra khóa ngoại, không xóa danh mục đang có sản phẩm hoặc sản phẩm đã có đơn.
- [x] **Format / Validate:** Data Annotation, ValidationSummary / ValidationMessageFor, định dạng giá VNĐ, số lượng nguyên dương, kiểm tra trạng thái.
- [x] **Search / Filter:** tìm theo từ khóa, danh mục, khoảng giá, trạng thái đơn.
- [x] **Tiền tăng/giảm khi đặt/hủy hàng:** tiền trong giỏ tính theo số lượng và lựa chọn; tổng tiền của đơn ghi snapshot lúc đặt; bảng thống kê không tính đơn đã hủy; đặt đơn trừ số lượng tồn kho, hủy đơn cộng lại tồn kho (không xóa lịch sử giao dịch).
- [x] **Giữ session khi login/logout:** `CartService` giữ session riêng `MyStore.Cart`, logout FormsAuthentication.SignOut **không** gọi Session.Abandon.

## Những gì vẫn cần môi trường Windows để nghiệm thu

- [ ] Mở `.sln`, restore NuGet, Rebuild Solution trên Visual Studio 2022.
- [ ] Kết nối SQL Server và chạy thử CRUD thật qua trình duyệt.
- [ ] Test đồng thời đặt/hủy đơn qua nhiều trình duyệt.
- [ ] Thử PayPal Sandbox bằng tài khoản test (nếu yêu cầu giảng viên có PayPal).
- [ ] Nếu bài chấm bắt buộc `.edmx`/`PagedList.Mvc`/Google Charts, thay đổi code theo đúng quy định chấm; bản hiện tại sử dụng cơ chế tương đương, chưa phải thư viện/code y hệt video.
