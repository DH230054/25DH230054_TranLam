# Bản sửa lỗi menu khi chạy website MyStore (ASP.NET MVC5)

## Vấn đề được xử lý

Ở bản trước, Visual Studio dừng ở dòng `@Html.Action("PersonalMenu", "Home", new { area = "" })`
trong `Views/Shared/_CustomerLayout.cshtml` và hiển thị thông báo chung `Error executing child request for handler`.
Thông báo chung đó không cho biết chính xác lỗi bên trong; bản này chủ động bỏ child request cho menu cá nhân.

- Dòng gọi `Html.Action("PersonalMenu", ...)` được thay bằng `Html.Partial("_PersonalMenuPV", ...)`.
- Thêm `Services/NavigationMenuService.cs` xây dựng dữ liệu menu ở ngay request hiện tại.
- Không làm mất giỏ hàng khi đăng nhập/đăng xuất; đọc số lượng từ Session nếu có.
- Vẫn hiển thị link Admin cho tài khoản có quyền A.
- Không thay đổi cấu trúc CSDL và không xóa dữ liệu.

## Cách thử

1. Trong Visual Studio, chọn **Debug > Stop Debugging** (Shift+F5) và đóng solution cũ.
2. Giải nén ZIP mới ra **một thư mục mới**, không giải nén chồng file vào thư mục cũ khi Visual Studio đang mở.
3. Mở `MyStore_MVC5_Full.sln` trong thư mục mới bằng Visual Studio 2022.
4. Nếu Visual Studio yêu cầu thì chạy **Restore NuGet Packages**.
5. Chọn `MyStore.Web` là **Startup Project**, rồi **Build > Rebuild Solution**.
6. Nhấn **F5** thử chạy lại website.

**KHÔNG CẦN chạy lại file tạo database / dữ liệu mẫu** nếu trước đó cửa sổ cài đặt đã hiển thị `SUCCESS - Database MyStore is ready. Products: 20`.
`Web.config` tiếp tục dùng `Data Source=(localdb)\MSSQLLocalDB`.

Nếu vẫn có lỗi: trong hộp **Exception User-Unhandled**, nhấn mũi tên **Inner exception** rồi chụp lại **thông báo lỗi bên trong**, hoặc nhấn **View Details** để gửi thông báo lỗi đầy đủ.
