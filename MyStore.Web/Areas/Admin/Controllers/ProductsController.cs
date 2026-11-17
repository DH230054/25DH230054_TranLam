using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class ProductsController : Controller
    {
        readonly MyStoreEntities db=new MyStoreEntities();
        private void SetCategories(int? selected=null)=>ViewBag.CategoryID=new SelectList(db.Categories.OrderBy(x=>x.CategoryName).ToList(),"CategoryID","CategoryName",selected);
        public ActionResult Index(string keyword,decimal? minPrice,decimal? maxPrice,string sortOrder,int page=1)
        {
            if(page<1)return new HttpStatusCodeResult(400);
            var vm=new ProductSearchVM{Keyword=keyword,MinPrice=minPrice,MaxPrice=maxPrice,SortOrder=sortOrder,Page=page,PageSize=10};
            if(minPrice<0||maxPrice<0||(minPrice.HasValue&&maxPrice.HasValue&&minPrice>maxPrice))
            {ModelState.AddModelError("","Khoảng giá không hợp lệ.");return View(vm);}
            var q=db.Products.Include(x=>x.Category).AsQueryable();
            if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.ProductName.Contains(keyword)||x.ProductDescription.Contains(keyword)||x.Category.CategoryName.Contains(keyword));
            if(minPrice.HasValue)q=q.Where(x=>x.ProductPrice>=minPrice.Value);
            if(maxPrice.HasValue)q=q.Where(x=>x.ProductPrice<=maxPrice.Value);
            vm.TotalCount=q.Count();
            switch(sortOrder)
            {
                case "name_asc":q=q.OrderBy(x=>x.ProductName).ThenBy(x=>x.ProductID);break;
                case "name_desc":q=q.OrderByDescending(x=>x.ProductName).ThenBy(x=>x.ProductID);break;
                case "price_asc":q=q.OrderBy(x=>x.ProductPrice).ThenBy(x=>x.ProductID);break;
                case "price_desc":q=q.OrderByDescending(x=>x.ProductPrice).ThenBy(x=>x.ProductID);break;
                default:q=q.OrderByDescending(x=>x.ProductID);break;
            }
            vm.Products=q.Skip((page-1)*vm.PageSize).Take(vm.PageSize).ToList();return View(vm);
        }
        public ActionResult Details(int id){var p=db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.ProductID==id);return p==null?(ActionResult)HttpNotFound():View(p);}
        public ActionResult Create(){SetCategories();return View(new Product());}
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CategoryID,ProductName,ProductDescription,ProductPrice,StockQuantity,UploadImg")]Product p)
        {
            if(!db.Categories.Any(x=>x.CategoryID==p.CategoryID))ModelState.AddModelError("CategoryID","Danh mục không hợp lệ.");
            if(!ModelState.IsValid){SetCategories(p.CategoryID);return View(p);}
            try{p.ProductImage=SaveImage(p.UploadImg);db.Products.Add(p);db.SaveChanges();return RedirectToAction("Index");}
            catch(ArgumentException e){ModelState.AddModelError("UploadImg",e.Message);SetCategories(p.CategoryID);return View(p);}
        }
        public ActionResult Edit(int id)
        {
            var p=db.Products.Find(id);if(p==null)return HttpNotFound();SetCategories(p.CategoryID);return View(p);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,StockQuantity,UploadImg")]Product p)
        {
            var old=db.Products.Find(p.ProductID);if(old==null)return HttpNotFound();
            if(!db.Categories.Any(x=>x.CategoryID==p.CategoryID))ModelState.AddModelError("CategoryID","Danh mục không hợp lệ.");
            if(!ModelState.IsValid){SetCategories(p.CategoryID);p.ProductImage=old.ProductImage;return View(p);}
            try
            {
                old.CategoryID=p.CategoryID;old.ProductName=p.ProductName;old.ProductDescription=p.ProductDescription;
                old.ProductPrice=p.ProductPrice;old.StockQuantity=p.StockQuantity;
                if(p.UploadImg!=null&&p.UploadImg.ContentLength>0) old.ProductImage=SaveImage(p.UploadImg);
                db.SaveChanges();return RedirectToAction("Index");
            }
            catch(ArgumentException e){ModelState.AddModelError("UploadImg",e.Message);SetCategories(p.CategoryID);return View(p);}
        }
        private string SaveImage(HttpPostedFileBase upload)
        {
            if(upload==null||upload.ContentLength==0)return null;
            if(upload.ContentLength>3*1024*1024)throw new ArgumentException("Ảnh phải nhỏ hơn 3 MB.");
            var ext=Path.GetExtension(upload.FileName).ToLowerInvariant();
            var okExt=new[]{".jpg",".jpeg",".png",".webp"};
            if(!okExt.Contains(ext))throw new ArgumentException("Chỉ nhận JPG, PNG, WEBP.");
            var mimetype=(upload.ContentType??"").ToLowerInvariant();
            if(!new[]{"image/jpeg","image/png","image/webp"}.Contains(mimetype))throw new ArgumentException("Định dạng ảnh không hợp lệ.");
            // Kiểm tra signature thật, không dựa riêng ContentType / đuôi file.
            var signature=new byte[12];var len=upload.InputStream.Read(signature,0,12);upload.InputStream.Position=0;
            bool jpg=len>=3&&signature[0]==0xff&&signature[1]==0xd8&&signature[2]==0xff;
            bool png=len>=8&&signature[0]==0x89&&signature[1]==0x50&&signature[2]==0x4e&&signature[3]==0x47;
            bool webp=len>=12&&System.Text.Encoding.ASCII.GetString(signature,0,4)=="RIFF"&&System.Text.Encoding.ASCII.GetString(signature,8,4)=="WEBP";
            if(!(jpg&& (ext==".jpg"||ext==".jpeg") || png&&ext==".png" || webp&&ext==".webp")) throw new ArgumentException("Tệp không phải ảnh hợp lệ.");
            var folder=Server.MapPath("~/Content/images");Directory.CreateDirectory(folder);
            var filename=Guid.NewGuid().ToString("N")+ext;
            upload.SaveAs(Path.Combine(folder,filename));return "~/Content/images/"+filename;
        }
        public ActionResult Delete(int id){var p=db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.ProductID==id);return p==null?(ActionResult)HttpNotFound():View(p);}
        [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var p=db.Products.Find(id);if(p==null)return HttpNotFound();
            if(db.OrderDetails.Any(x=>x.ProductID==id)){TempData["Error"]="Sản phẩm đã có lịch sử đơn hàng, không thể xóa.";return RedirectToAction("Index");}
            db.Products.Remove(p);db.SaveChanges();return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
