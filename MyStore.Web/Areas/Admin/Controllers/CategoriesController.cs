using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class CategoriesController : Controller
    {
        readonly MyStoreEntities db=new MyStoreEntities();
        public ActionResult Index(string keyword)
        {
            var q=db.Categories.AsQueryable();if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.CategoryName.Contains(keyword));
            ViewBag.Keyword=keyword;return View(q.OrderBy(x=>x.CategoryID).ToList());
        }
        public ActionResult Details(int id){var c=db.Categories.Find(id);return c==null?(ActionResult)HttpNotFound():View(c);}
        public ActionResult Create()=>View(new Category());
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CategoryName")]Category c)
        {
            if(!ModelState.IsValid)return View(c);
            if(db.Categories.Any(x=>x.CategoryName==c.CategoryName)){ModelState.AddModelError("CategoryName","Tên danh mục đã tồn tại.");return View(c);}
            db.Categories.Add(c);db.SaveChanges();return RedirectToAction("Index");
        }
        public ActionResult Edit(int id){var c=db.Categories.Find(id);return c==null?(ActionResult)HttpNotFound():View(c);}
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CategoryID,CategoryName")]Category c)
        {
            if(!ModelState.IsValid)return View(c);
            var old=db.Categories.Find(c.CategoryID);if(old==null)return HttpNotFound();
            if(db.Categories.Any(x=>x.CategoryID!=c.CategoryID&&x.CategoryName==c.CategoryName)){ModelState.AddModelError("CategoryName","Tên danh mục đã tồn tại.");return View(c);}
            old.CategoryName=c.CategoryName;db.SaveChanges();return RedirectToAction("Index");
        }
        public ActionResult Delete(int id){var c=db.Categories.Find(id);return c==null?(ActionResult)HttpNotFound():View(c);}
        [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var c=db.Categories.Find(id);if(c==null)return HttpNotFound();
            if(db.Products.Any(x=>x.CategoryID==id)){TempData["Error"]="Danh mục đang có sản phẩm, không thể xóa.";return RedirectToAction("Index");}
            db.Categories.Remove(c);db.SaveChanges();return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
