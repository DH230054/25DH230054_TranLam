using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MyStore.Web.Models;
namespace MyStore.Web.Services
{
    public static class OrderService
    {
        public static StoreOrder Place(MyStoreEntities db, Cart cart, Customer customer, CheckoutVM vm, bool paid = false)
        {
            var items = cart.Items.Where(x => x.Selected).ToList();
            if (!items.Any()) throw new InvalidOperationException("Chưa chọn sản phẩm để đặt hàng.");
            if (items.Any(x => x.Quantity < 1 || x.Quantity > 1000)) throw new InvalidOperationException("Số lượng không hợp lệ.");
            if (vm.PaymentMethod != "COD" && vm.PaymentMethod != "PayPal") throw new InvalidOperationException("Hình thức thanh toán không hợp lệ.");
            if (!ShippingOptions.IsValid(vm.DeliveryMethod)) throw new InvalidOperationException("Hình thức giao hàng không hợp lệ.");
            using (var tx = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var ids = items.Select(x => x.ProductID).ToList();
                var products = db.Products.Where(x => ids.Contains(x.ProductID)).ToList();
                if (products.Count != ids.Distinct().Count()) throw new InvalidOperationException("Có sản phẩm không còn tồn tại.");
                // Atomic stock decrement avoids negative inventory under concurrent orders.
                foreach (var item in items)
                {
                    var updated = db.Database.ExecuteSqlCommand(
                        "UPDATE dbo.Product SET StockQuantity=StockQuantity-@p0 WHERE ProductID=@p1 AND StockQuantity>=@p0",
                        item.Quantity, item.ProductID);
                    if (updated != 1) throw new InvalidOperationException("Sản phẩm " + item.ProductName + " không đủ tồn kho.");
                }
                var order = new StoreOrder
                {
                    CustomerID = customer.CustomerID, OrderDate = DateTime.Now,
                    PaymentMethod = vm.PaymentMethod, PaymentStatus = paid ? "Paid" : "Pending",
                    DeliveryMethod = vm.DeliveryMethod, ShippingAddress = vm.ShippingAddress.Trim(), OrderStatus = "Pending"
                };
                foreach (var item in items)
                {
                    var p = products.First(x => x.ProductID == item.ProductID);
                    order.OrderDetails.Add(new OrderDetail { ProductID = p.ProductID, Quantity = item.Quantity, UnitPrice = p.ProductPrice });
                }
                var subtotal = order.OrderDetails.Sum(x => x.Quantity * x.UnitPrice);
                order.ShippingFee = ShippingOptions.Fee(vm.DeliveryMethod, subtotal);
                order.TotalAmount = subtotal + order.ShippingFee;
                db.Orders.Add(order); db.SaveChanges(); tx.Commit();
                return order;
            }
        }
        public static void Cancel(MyStoreEntities db, StoreOrder order)
        {
            if (order == null || order.OrderStatus != "Pending") throw new InvalidOperationException("Chỉ có thể hủy đơn đang chờ xử lý.");
            using (var tx = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                // Re-read and lock order row to block simultaneous cancellation.
                var status = db.Database.SqlQuery<string>(
                    "SELECT OrderStatus FROM dbo.[Order] WITH (UPDLOCK, HOLDLOCK) WHERE OrderID=@p0", order.OrderID).Single();
                if (status != "Pending") throw new InvalidOperationException("Đơn hàng đã đổi trạng thái.");
                var details = db.OrderDetails.Where(x => x.OrderID == order.OrderID).ToList();
                foreach (var d in details) db.Database.ExecuteSqlCommand(
                    "UPDATE dbo.Product SET StockQuantity=StockQuantity+@p0 WHERE ProductID=@p1", d.Quantity, d.ProductID);
                order.OrderStatus = "Cancelled";
                order.PaymentStatus = order.PaymentStatus == "Paid" ? "RefundPending" : "Cancelled";
                db.SaveChanges(); tx.Commit();
            }
        }
    }
}
