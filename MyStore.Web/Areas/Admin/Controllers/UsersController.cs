using System;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class UsersController:Controller
    {
        readonly MyStoreEntities db=new MyStoreEntities();
        public ActionResult Index(string keyword,int page=1)
        {
            if(page<1)return new HttpStatusCodeResult(400);
            var q=db.Users.Where(x=>x.UserRole=="A");
            if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.Username.Contains(keyword));
            ViewBag.Keyword=keyword;ViewBag.Page=page;ViewBag.Total=q.Count();
            return View(q.OrderBy(x=>x.Username).Skip((page-1)*10).Take(10).ToList());
        }
        public ActionResult Create()=>View(new AdminUserVM());
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create(AdminUserVM vm)
        {
            if(!string.IsNullOrWhiteSpace(vm.Username) && db.Users.Any(x=>x.Username==vm.Username))ModelState.AddModelError("Username","Tên đăng nhập đã tồn tại.");
            if(!ModelState.IsValid)return View(vm);
            db.Users.Add(new StoreUser{Username=vm.Username,Password=PasswordHasher.Hash(vm.Password),UserRole="A"});
            db.SaveChanges();return RedirectToAction("Index");
        }
        public ActionResult Edit(string id)
        {
            var u=db.Users.Find(id);if(u==null||u.UserRole!="A")return HttpNotFound();
            ViewBag.Username=u.Username;return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit(string id,string newPassword)
        {
            var u=db.Users.Find(id);if(u==null||u.UserRole!="A")return HttpNotFound();
            if(string.IsNullOrWhiteSpace(newPassword)||newPassword.Length<8||newPassword.Length>80)
            {ModelState.AddModelError("","Mật khẩu mới phải từ 8 đến 80 ký tự.");ViewBag.Username=id;return View();}
            u.Password=PasswordHasher.Hash(newPassword);db.SaveChanges();return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            var u=db.Users.Find(id);if(u==null||u.UserRole!="A")return HttpNotFound();return View(u);
        }
        [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var u=db.Users.Find(id);if(u==null||u.UserRole!="A")return HttpNotFound();
            if(id==User.Identity.Name){TempData["Error"]="Không thể xóa tài khoản đang đăng nhập.";return RedirectToAction("Index");}
            if(db.Users.Count(x=>x.UserRole=="A")<=1){TempData["Error"]="Không thể xóa quản trị viên cuối cùng.";return RedirectToAction("Index");}
            if(db.Customers.Any(x=>x.Username==id)){TempData["Error"]="Tài khoản quản trị này có hồ sơ khách hàng. Không thể xóa trực tiếp do ràng buộc dữ liệu.";return RedirectToAction("Index");}
            db.Users.Remove(u);db.SaveChanges();return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
