using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        [HttpGet, AllowAnonymous] public ActionResult Register() => View(new RegisterVM());
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM vm)
        {
            if (vm.Username != null && db.Users.Any(x=>x.Username==vm.Username)) ModelState.AddModelError("Username","Tên đăng nhập đã tồn tại.");
            if (!ModelState.IsValid) return View(vm);
            using(var tx=db.Database.BeginTransaction())
            {
                db.Users.Add(new StoreUser { Username=vm.Username, Password=PasswordHasher.Hash(vm.Password), UserRole="C" });
                db.Customers.Add(new Customer { Username=vm.Username, CustomerName=vm.CustomerName.Trim(), CustomerPhone=vm.CustomerPhone, CustomerEmail=vm.CustomerEmail, CustomerAddress=vm.CustomerAddress });
                db.SaveChanges();tx.Commit();
            }
            FormsAuthentication.SetAuthCookie(vm.Username,false);
            return RedirectToAction("Index","Home");
        }
        [HttpGet, AllowAnonymous] public ActionResult Login(string returnUrl) => View(new LoginVM{ReturnUrl=returnUrl});
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm)
        {
            if(!ModelState.IsValid)return View(vm);
            var user=db.Users.Find(vm.Username);
            if(user==null || !PasswordHasher.Verify(vm.Password,user.Password))
            {ModelState.AddModelError("","Sai tên đăng nhập hoặc mật khẩu.");return View(vm);}
            FormsAuthentication.SetAuthCookie(user.Username,false);
            if(Url.IsLocalUrl(vm.ReturnUrl))return Redirect(vm.ReturnUrl);
            return user.UserRole == "A" ? RedirectToAction("Index","Home",new{area="Admin"}) : RedirectToAction("Index","Home");
        }
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // Giữ nguyên Session[MyStore.Cart]; KHÔNG gọi Session.Abandon().
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }
        [Authorize]
        public ActionResult ProfileInfo()
        {
            var c=db.Customers.FirstOrDefault(x=>x.Username==User.Identity.Name);
            if(c==null)return new HttpStatusCodeResult(403);
            return View(new ProfileVM{CustomerName=c.CustomerName,CustomerPhone=c.CustomerPhone,CustomerEmail=c.CustomerEmail,CustomerAddress=c.CustomerAddress});
        }
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(ProfileVM vm)
        {
            if(!ModelState.IsValid){return View("ProfileInfo",vm);}
            var c=db.Customers.FirstOrDefault(x=>x.Username==User.Identity.Name);
            if(c==null)return new HttpStatusCodeResult(403);
            c.CustomerName=vm.CustomerName.Trim();c.CustomerPhone=vm.CustomerPhone;c.CustomerEmail=vm.CustomerEmail;c.CustomerAddress=vm.CustomerAddress;
            db.SaveChanges();TempData["Success"]="Cập nhật thông tin thành công.";return RedirectToAction("ProfileInfo");
        }
        [Authorize] public ActionResult ChangePassword() => View(new ChangePasswordVM());
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM vm)
        {
            if(!ModelState.IsValid)return View(vm);
            var user=db.Users.Find(User.Identity.Name);
            if(user==null || !PasswordHasher.Verify(vm.OldPassword,user.Password))
            { ModelState.AddModelError("OldPassword","Mật khẩu hiện tại không đúng.");return View(vm); }
            user.Password=PasswordHasher.Hash(vm.NewPassword);db.SaveChanges();
            TempData["Success"]="Đổi mật khẩu thành công.";return RedirectToAction("ProfileInfo");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
