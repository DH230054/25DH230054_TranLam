# MY STORE – DA SUA LOI TIENG VIET VA GIO HANG (17/11/2026)

## Loi 1 – Tieng Viet hien thi thanh ky tu la
- Trong ban truoc, `Views/Home/Index.cshtml` va nhieu file Razor chua co dau UTF-8 BOM.
- Da luu tat ca **45 file .cshtml** va **22 file .cs** thanh UTF-8 co BOM.
- Da them `fileEncoding="utf-8"` vao `globalization` trong `MyStore.Web/Web.config`.
- Da giu `responseEncoding="utf-8"` va `<meta charset="utf-8">` trong layout.
- Font Segoe UI duoc giu nguyen; day la loi **ma hoa**, khong phai thieu font.

## Loi 2 – Them san pham vao gio hang thi trang gio hang loi
- Da sua `MyStore.Web/Views/Cart/Index.cshtml`.
- Sua loi Razor o khoi `@if (Model.SimilarProducts.Any())` nam ben trong `else`.
- Viet lai cau truc `if/else`, gom san pham theo danh muc, form chon / sua so luong / xoa.
- Moi form POST van co `@Html.AntiForgeryToken()`; khong thay doi API CartController.

## Cach mo lai
1. Trong Visual Studio cu, nhan Shift+F5, dong Visual Studio.
2. Giai nen ban ZIP nay vao **thu muc moi**, vi du `D:\MyStore_FontGioHang`.
3. Mo `MyStore_MVC5_Full.sln` trong thu muc moi bang Visual Studio 2022.
4. Build -> Clean Solution; Build -> Rebuild Solution; bam F5.
5. Trong trinh duyet bam Ctrl+F5 de tai lai trang, Ctrl+0 de ve muc zoom 100%.
6. Thu: trang chu co dau; bam them gio; gio hang; thay doi so luong; xoa mon.

**KHONG** can chay lai file SQL. LocalDB `(localdb)\MSSQLLocalDB` duoc giu nguyen.

Da kiem tra ma hoa, cu phap co ban va tinh toan ven file ZIP.
Chua the mo Visual Studio 2022/IIS Express tren Windows de xac nhan build/runtime.
