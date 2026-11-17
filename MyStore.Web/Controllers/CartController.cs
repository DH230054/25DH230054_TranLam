using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        private CartService Cart => new CartService(Session);
        public ActionResult Index()
        {
            Cart.SyncPrices(db);
            var vm=Cart.GetCart();
            var ids=vm.Items.Select(i=>i.ProductID).ToList();
            var categoryIds=db.Products.Where(x=>ids.Contains(x.ProductID)).Select(x=>x.CategoryID).Distinct().ToList();
            vm.SimilarProducts=db.Products.Where(x=>categoryIds.Contains(x.CategoryID)&&!ids.Contains(x.ProductID)).OrderByDescending(x=>x.ProductID).Take(6).ToList();
            return View(vm);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult AddItem(int productId, int quantity=1)
        {
            var p=db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.ProductID==productId);
            if(p==null)return HttpNotFound();
            try {Cart.Add(p,p.Category.CategoryName,quantity);TempData["Success"]="Đã thêm vào giỏ hàng.";}
            catch(Exception e)when(e is ArgumentException||e is InvalidOperationException){TempData["Error"]=e.Message;}
            return RedirectToAction("Index");
        }
        [HttpPost,ValidateAntiForgeryToken] public ActionResult RemoveItem(int productId){Cart.Remove(productId);return RedirectToAction("Index");}
        [HttpPost,ValidateAntiForgeryToken] public ActionResult ClearCart(){Cart.Clear();return RedirectToAction("Index");}
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart=Cart.GetCart();var i=cart.Items.FirstOrDefault(x=>x.ProductID==productId);
            if(i==null)return HttpNotFound();
            var p=db.Products.Find(productId);
            if(p==null)return HttpNotFound();
            if(quantity<1 || quantity>Math.Min(1000,p.StockQuantity)) TempData["Error"]="Số lượng không hợp lệ hoặc vượt tồn kho.";
            else i.Quantity=quantity;
            return RedirectToAction("Index");
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult SelectItem(int productId,bool selected=false)
        {
            var item=Cart.GetCart().Items.FirstOrDefault(x=>x.ProductID==productId);
            if(item!=null)item.Selected=selected;
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
