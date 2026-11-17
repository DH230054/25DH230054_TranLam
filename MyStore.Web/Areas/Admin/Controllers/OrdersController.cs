using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class OrdersController : Controller
    {
        readonly MyStoreEntities db=new MyStoreEntities();
        public ActionResult Index(string customerName, string paymentStatus, DateTime? orderDate,
            string sort="newest", int page=1, string keyword=null, string status=null)
        {
            if(page<1)return new HttpStatusCodeResult(400);
            if(string.IsNullOrWhiteSpace(customerName))customerName=keyword;
            var q=db.Orders.Include(x=>x.Customer).AsQueryable();
            if(!string.IsNullOrWhiteSpace(customerName)){
                var name=customerName.Trim();
                q=q.Where(x=>x.Customer.CustomerName.Contains(name));
            }
            if(!string.IsNullOrWhiteSpace(paymentStatus))q=q.Where(x=>x.PaymentStatus==paymentStatus);
            if(orderDate.HasValue){var date=orderDate.Value.Date;q=q.Where(x=>x.OrderDate==date);}
            if(!string.IsNullOrWhiteSpace(status))q=q.Where(x=>x.OrderStatus==status);
            ViewBag.CustomerName=customerName;
            ViewBag.PaymentStatus=paymentStatus;
            ViewBag.OrderDate=orderDate.HasValue ? orderDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.Sort=sort;
            ViewBag.Status=status;
            ViewBag.Page=page;
            ViewBag.Total=q.Count();
            switch(sort){
                case "oldest":q=q.OrderBy(x=>x.OrderDate).ThenBy(x=>x.OrderID);break;
                case "total_asc":q=q.OrderBy(x=>x.TotalAmount).ThenByDescending(x=>x.OrderID);break;
                case "total_desc":q=q.OrderByDescending(x=>x.TotalAmount).ThenByDescending(x=>x.OrderID);break;
                case "customer":q=q.OrderBy(x=>x.Customer.CustomerName).ThenByDescending(x=>x.OrderID);break;
                default:q=q.OrderByDescending(x=>x.OrderDate).ThenByDescending(x=>x.OrderID);break;
            }
            // 15 don hang / trang (yeu cau slide 5).
            return View(q.Skip((page-1)*15).Take(15).ToList());
        }
        public ActionResult DetailedOrder(int id)
        {
            var o=db.Orders.Include(x=>x.Customer).Include(x=>x.OrderDetails.Select(d=>d.Product)).FirstOrDefault(x=>x.OrderID==id);
            return o==null?(ActionResult)HttpNotFound():View(o);
        }
        public ActionResult Details(int id) => RedirectToAction("DetailedOrder",new{id});
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            var order=db.Orders.Include(x=>x.Customer).FirstOrDefault(x=>x.OrderID==id);
            return order==null?(ActionResult)HttpNotFound():View(order);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id,string orderStatus,string paymentStatus)
        {
            var o=db.Orders.Find(id);if(o==null)return HttpNotFound();
            var allowed=new[]{"Pending","Processing","Shipping","Delivered"};
            if(!allowed.Contains(orderStatus))return new HttpStatusCodeResult(400,"Trạng thái đơn không hợp lệ.");
            if(o.OrderStatus=="Cancelled"||o.OrderStatus=="Delivered")return new HttpStatusCodeResult(409,"Đơn đã giao hoặc đã hủy không thể cập nhật.");
            if(Array.IndexOf(allowed,orderStatus)<Array.IndexOf(allowed,o.OrderStatus))return new HttpStatusCodeResult(409,"Không thể quay ngược trạng thái.");
            if(paymentStatus!="Pending" && paymentStatus!="Paid")return new HttpStatusCodeResult(400,"Trạng thái thanh toán không hợp lệ.");
            if(o.PaymentStatus=="RefundPending"||o.PaymentStatus=="Cancelled")return new HttpStatusCodeResult(409,"Không thể sửa thanh toán của đơn đang hoàn/hủy.");
            if(o.PaymentStatus=="Paid" && paymentStatus!="Paid")return new HttpStatusCodeResult(409,"Đơn đã thanh toán, không thể đánh dấu chưa thanh toán.");
            o.OrderStatus=orderStatus;
            o.PaymentStatus=paymentStatus;
            db.SaveChanges();
            TempData["Success"]="Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction("DetailedOrder",new{id});
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult SetStatus(int id,string status)
        {
            var o=db.Orders.Find(id);if(o==null)return HttpNotFound();
            var allowed=new[]{"Pending","Processing","Shipping","Delivered"};
            if(!allowed.Contains(status))return new HttpStatusCodeResult(400);
            if(o.OrderStatus=="Cancelled"||o.OrderStatus=="Delivered")return new HttpStatusCodeResult(409);
            if(Array.IndexOf(allowed,status)<Array.IndexOf(allowed,o.OrderStatus))return new HttpStatusCodeResult(409);
            o.OrderStatus=status;db.SaveChanges();return RedirectToAction("DetailedOrder",new{id});
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var o=db.Orders.Find(id);if(o==null)return HttpNotFound();
            try{OrderService.Cancel(db,o);TempData["Success"]="Đã hủy và cộng lại tồn kho.";}
            catch(InvalidOperationException e){TempData["Error"]=e.Message;}
            return RedirectToAction("DetailedOrder",new{id});
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
