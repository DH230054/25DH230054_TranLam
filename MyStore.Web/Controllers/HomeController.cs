using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index()
        {
            return View(new HomeProductVM
            {
                Featured = db.Products.Include(x=>x.Category).OrderByDescending(x=>x.OrderDetails.Count).ThenBy(x=>x.ProductID).Take(8).ToList(),
                NewProducts = db.Products.Include(x=>x.Category).OrderByDescending(x=>x.ProductID).Take(12).ToList(),
                Categories = db.Categories.OrderBy(x=>x.CategoryName).ToList()
            });
        }
        [ChildActionOnly] public ActionResult CategoryMenu() => PartialView("_CategoriesPV", db.Categories.OrderBy(x=>x.CategoryName).ToList());
        [ChildActionOnly] public ActionResult PersonalMenu()
        {
            var isLogged = User.Identity.IsAuthenticated;
            var username = isLogged ? User.Identity.Name : null;
            return PartialView("_PersonalMenuPV",new PersonalMenuVM
            {
                IsLoggedIn=isLogged, Username=username,
                IsAdmin=isLogged && db.Users.Any(x=>x.Username==username && x.UserRole=="A"),
                CartCount=new CartService(Session).GetCart().Count
            });
        }
        public ActionResult ProductList(string keyword, int? categoryId, decimal? minPrice, decimal? maxPrice, string sortOrder, int page = 1)
        {
            var vm = new ProductSearchVM { Keyword=keyword, CategoryID=categoryId, MinPrice=minPrice,MaxPrice=maxPrice,SortOrder=sortOrder,Page=page,PageSize=9 };
            if (page < 1) return new HttpStatusCodeResult(400);
            ViewBag.Categories = new SelectList(db.Categories.OrderBy(x=>x.CategoryName).ToList(),"CategoryID","CategoryName",categoryId);
            if (minPrice < 0 || maxPrice < 0 || (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice))
            { ModelState.AddModelError("", "Khoảng giá không hợp lệ."); return View(vm); }
            var q=db.Products.Include(x=>x.Category).AsQueryable();
            if(!string.IsNullOrWhiteSpace(keyword)) { var kw=keyword.Trim();q=q.Where(x=>x.ProductName.Contains(kw)||x.ProductDescription.Contains(kw)||x.Category.CategoryName.Contains(kw)); }
            if(categoryId.HasValue)q=q.Where(x=>x.CategoryID==categoryId.Value);
            if(minPrice.HasValue)q=q.Where(x=>x.ProductPrice>=minPrice.Value);
            if(maxPrice.HasValue)q=q.Where(x=>x.ProductPrice<=maxPrice.Value);
            vm.TotalCount=q.Count();
            switch (sortOrder)
            {
                case "name_desc": q=q.OrderByDescending(x=>x.ProductName).ThenBy(x=>x.ProductID);break;
                case "name_asc": q=q.OrderBy(x=>x.ProductName).ThenBy(x=>x.ProductID);break;
                case "price_asc": q=q.OrderBy(x=>x.ProductPrice).ThenBy(x=>x.ProductID);break;
                case "price_desc": q=q.OrderByDescending(x=>x.ProductPrice).ThenBy(x=>x.ProductID);break;
                default: q=q.OrderByDescending(x=>x.ProductID);break;
            }
            vm.Products=q.Skip((page-1)*vm.PageSize).Take(vm.PageSize).ToList();
            ViewBag.Categories = new SelectList(db.Categories.OrderBy(x=>x.CategoryName).ToList(),"CategoryID","CategoryName",categoryId);
            return View(vm);
        }
        public ActionResult ProductDetail(int id)
        {
            var p=db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.ProductID==id);
            if(p==null)return HttpNotFound();
            return View(new ProductDetailsVM{ Product=p,
                RelatedProducts=db.Products.Where(x=>x.CategoryID==p.CategoryID && x.ProductID!=id).OrderByDescending(x=>x.ProductID).Take(4).ToList() });
        }
        protected override void Dispose(bool disposing){if(disposing) db.Dispose();base.Dispose(disposing);}
    }
}
