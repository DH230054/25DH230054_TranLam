using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        private CartService Cart => new CartService(Session);
        private Customer CurrentCustomer() => db.Customers.FirstOrDefault(x=>x.Username==User.Identity.Name);
        [HttpGet]
        public ActionResult Checkout()
        {
            Cart.SyncPrices(db);var cart=Cart.GetCart();
            if(!cart.Items.Any(x=>x.Selected))return RedirectToAction("Index","Cart");
            var customer=CurrentCustomer();if(customer==null)return new HttpStatusCodeResult(403);
            return View(new CheckoutVM{ShippingAddress=customer.CustomerAddress,Items=cart.Items.Where(x=>x.Selected).ToList(),TotalAmount=cart.Total,ShippingFee=ShippingOptions.Fee("Standard",cart.Total)});
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM vm)
        {
            Cart.SyncPrices(db);var cart=Cart.GetCart();
            vm.Items=cart.Items.Where(x=>x.Selected).ToList();vm.TotalAmount=cart.Total;
            vm.ShippingFee = ShippingOptions.IsValid(vm.DeliveryMethod) ? ShippingOptions.Fee(vm.DeliveryMethod, cart.Total) : 0m;
            var customer=CurrentCustomer();if(customer==null)return new HttpStatusCodeResult(403);
            if(!ModelState.IsValid)return View(vm);
            if(vm.PaymentMethod=="PayPal")
            {
                // Không giả mạo kết quả PayPal. Kích hoạt luồng sandbox REST riêng.
                Session["MyStore.PayPalCheckout"] = new CheckoutVM{
                    ShippingAddress=vm.ShippingAddress, PaymentMethod="PayPal", DeliveryMethod=vm.DeliveryMethod
                };
                return RedirectToAction("PaymentWithPaypal","Paypal");
            }
            try
            {
                var order=OrderService.Place(db,cart,customer,vm);
                Cart.RemovePurchased(order.OrderDetails.Select(x=>x.ProductID));
                return RedirectToAction("OrderSuccess",new{id=order.OrderID});
            }
            catch(InvalidOperationException e) {ModelState.AddModelError("",e.Message);return View(vm);}
        }
        public ActionResult OrderSuccess(int id)
        {
            var customer=CurrentCustomer();if(customer==null)return new HttpStatusCodeResult(403);
            var order=db.Orders.Include(x=>x.OrderDetails.Select(d=>d.Product))
                .FirstOrDefault(x=>x.OrderID==id&&x.CustomerID==customer.CustomerID);
            if(order==null)return HttpNotFound();return View(order);
        }
        // PowerPoint "Ktra BE": nhom 5 trang thai, moi nhom 2 don moi nhat;
        // nhan Xem them de xem toi da 50 don moi nhat cua nhom do.
        public ActionResult MyOrder(string status, string keyword, string expand)
        {
            var customer = CurrentCustomer();
            if (customer == null) return new HttpStatusCodeResult(403);
            var types = new[] {
                new { Code="Pending", Title="Chờ thanh toán" },
                new { Code="Processing", Title="Đang xử lý" },
                new { Code="Shipping", Title="Đang vận chuyển" },
                new { Code="Delivered", Title="Đã giao" },
                new { Code="Cancelled", Title="Đã hủy" }
            };
            if (!string.IsNullOrWhiteSpace(status) && !types.Any(t => t.Code == status)) return new HttpStatusCodeResult(400);
            var baseQuery = db.Orders.Where(x => x.CustomerID == customer.CustomerID);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                int orderId;
                if (int.TryParse(k, out orderId))
                    baseQuery = baseQuery.Where(x => x.OrderID == orderId || x.OrderDetails.Any(d => d.Product.ProductName.Contains(k)));
                else
                    baseQuery = baseQuery.Where(x => x.OrderDetails.Any(d => d.Product.ProductName.Contains(k)));
            }
            var vm = new OrderHistoryVM { Status=status ?? "", Keyword=keyword, Expand=expand };
            foreach (var t in types)
            {
                if (!string.IsNullOrWhiteSpace(status) && status != t.Code) continue;
                var q = baseQuery.Where(x => x.OrderStatus == t.Code);
                var total = q.Count();
                var expanded = string.Equals(expand, t.Code, StringComparison.Ordinal);
                var orders = q.Include(x=>x.OrderDetails.Select(d=>d.Product))
                    .OrderByDescending(x=>x.OrderDate).ThenByDescending(x=>x.OrderID)
                    .Take(expanded ? 50 : 2).ToList();
                vm.Groups.Add(new OrderStatusGroupVM {
                    Code=t.Code, Title=t.Title, Expanded=expanded, TotalCount=total, Orders=orders
                });
            }
            return View(vm);
        }
        public ActionResult OrderHistory(string status, string keyword) => RedirectToAction("MyOrder",new {status, keyword});

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BuyAgain(int productId)
        {
            var customer = CurrentCustomer();
            if(customer == null) return new HttpStatusCodeResult(403);
            // Mua lai chi san pham da tung mua, khong cho mua hang khong thuoc lich su.
            if(!db.OrderDetails.Any(d => d.ProductID == productId && d.Order.CustomerID == customer.CustomerID))
                return new HttpStatusCodeResult(403);
            var product = db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.ProductID==productId);
            if(product == null || product.StockQuantity < 1) {
                TempData["Error"] = "Sản phẩm này hiện đã hết hàng hoặc không còn bán.";
                return RedirectToAction("MyOrder");
            }
            try {
                Cart.Add(product, product.Category == null ? "Khác" : product.Category.CategoryName, 1);
                TempData["Success"] = "Đã thêm 1 sản phẩm vào giỏ hàng.";
                return RedirectToAction("Index", "Cart");
            }
            catch (InvalidOperationException e) {
                TempData["Error"] = e.Message;
                return RedirectToAction("MyOrder");
            }
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var customer=CurrentCustomer();if(customer==null)return new HttpStatusCodeResult(403);
            var order=db.Orders.FirstOrDefault(x=>x.OrderID==id&&x.CustomerID==customer.CustomerID);
            if(order==null)return HttpNotFound();
            try{OrderService.Cancel(db,order);TempData["Success"]="Đã hủy đơn và hoàn lại tồn kho.";}
            catch(InvalidOperationException e){TempData["Error"]=e.Message;}
            return RedirectToAction("MyOrder");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
