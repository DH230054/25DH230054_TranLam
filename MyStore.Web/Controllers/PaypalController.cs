using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Controllers
{
    [Authorize]
    public class PaypalController : Controller
    {
        private readonly MyStoreEntities db=new MyStoreEntities();
        private CartService Cart=>new CartService(Session);
        private CheckoutVM CheckoutInfo => Session["MyStore.PayPalCheckout"] as CheckoutVM;
        public async Task<ActionResult> PaymentWithPaypal()
        {
            var info=CheckoutInfo;
            if(info==null)return RedirectToAction("Checkout","Order");
            if(!PayPalService.Configured){TempData["Error"]="Chưa cấu hình thông tin tài khoản PayPal Sandbox. Hãy chọn COD hoặc cài đặt PayPal trong Web.config.";return RedirectToAction("Checkout","Order");}
            Cart.SyncPrices(db);
            var cart=Cart.GetCart();
            if(!cart.Items.Any(x=>x.Selected))return RedirectToAction("Index","Cart");
            try
            {
                var total=cart.Total + ShippingOptions.Fee(info.DeliveryMethod, cart.Total);
                var urls=await PayPalService.Create(total,Url.Action("Success","Paypal",null,Request.Url.Scheme),Url.Action("Cancel","Paypal",null,Request.Url.Scheme));
                Session["MyStore.PayPalID"]=urls.Item1;
                Session["MyStore.PayPalTotal"]=total;
                Session["MyStore.PayPalItems"]=string.Join(";",cart.Items.Where(x=>x.Selected).OrderBy(x=>x.ProductID).Select(x=>x.ProductID+":"+x.Quantity));
                return Redirect(urls.Item2);
            }
            catch(Exception e) {TempData["Error"]="Không thể tạo thanh toán PayPal: "+e.Message;return RedirectToAction("Checkout","Order");}
        }
        public async Task<ActionResult> Success(string token)
        {
            var info=CheckoutInfo;
            var id=Session["MyStore.PayPalID"] as string;
            if(info==null||string.IsNullOrWhiteSpace(id)||id!=token)return new HttpStatusCodeResult(400,"Thông tin giao dịch PayPal không hợp lệ.");
            Cart.SyncPrices(db);var cart=Cart.GetCart();
            var snapshot=string.Join(";",cart.Items.Where(x=>x.Selected).OrderBy(x=>x.ProductID).Select(x=>x.ProductID+":"+x.Quantity));
            if(snapshot!=(string)Session["MyStore.PayPalItems"]||(cart.Total + ShippingOptions.Fee(info.DeliveryMethod,cart.Total))!=(decimal)Session["MyStore.PayPalTotal"])
            {TempData["Error"]="Giá hoặc giỏ hàng đã thay đổi. Chưa xác nhận thu tiền. Hãy thanh toán lại.";return RedirectToAction("Checkout","Order");}
            try
            {
                var ok=await PayPalService.Capture(id,cart.Total + ShippingOptions.Fee(info.DeliveryMethod,cart.Total));
                if(!ok)throw new InvalidOperationException("PayPal chưa xác nhận hoàn tất giao dịch.");
                var customer=db.Customers.FirstOrDefault(x=>x.Username==User.Identity.Name);
                if(customer==null)throw new InvalidOperationException("Không tìm thấy tài khoản khách hàng.");
                // Thông báo đầy đủ khi lỗi lưu DB sau khi tiền đã được PayPal capture.
                var order=OrderService.Place(db,cart,customer,info,true);
                Cart.RemovePurchased(order.OrderDetails.Select(x=>x.ProductID));
                Session.Remove("MyStore.PayPalCheckout");Session.Remove("MyStore.PayPalID");Session.Remove("MyStore.PayPalTotal");Session.Remove("MyStore.PayPalItems");
                return RedirectToAction("OrderSuccess","Order",new{id=order.OrderID});
            }
            catch(Exception e)
            {
                // Không tự thử capture lại. Giữ token để admin đối soát thủ công.
                TempData["Error"]="Thanh toán chưa thể ghi nhận đơn. Hãy kiểm tra giao dịch PayPal '"+id+"' trước khi thử lại. "+e.Message;
                return RedirectToAction("Checkout","Order");
            }
        }
        public ActionResult Cancel()
        {
            Session.Remove("MyStore.PayPalCheckout");Session.Remove("MyStore.PayPalID");Session.Remove("MyStore.PayPalTotal");Session.Remove("MyStore.PayPalItems");
            TempData["Error"]="Bạn đã hủy bước thanh toán qua PayPal.";return RedirectToAction("Checkout","Order");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
