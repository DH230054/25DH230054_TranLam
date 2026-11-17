using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStore.Web.Models;
namespace MyStore.Web.Services
{
    public class CartService
    {
        private readonly HttpSessionStateBase session;
        public CartService(HttpSessionStateBase session) { this.session = session; }
        public Cart GetCart()
        {
            var cart = session["MyStore.Cart"] as Cart;
            if (cart == null) { cart = new Cart(); session["MyStore.Cart"] = cart; }
            return cart;
        }
        public void Add(Product p, string category, int qty)
        {
            if (qty < 1 || qty > 1000) throw new ArgumentException("Số lượng không hợp lệ.");
            var cart = GetCart(); var item = cart.Items.FirstOrDefault(x => x.ProductID == p.ProductID);
            if (item == null)
            {
                item = new CartItem { ProductID = p.ProductID, Quantity = 0, Selected=true };
                cart.Items.Add(item);
            }
            if ((long)item.Quantity + qty > p.StockQuantity) throw new InvalidOperationException("Số lượng vượt quá tồn kho.");
            item.Quantity += qty;
            item.ProductName = p.ProductName; item.UnitPrice = p.ProductPrice; item.ProductImage = ProductImageHelper.Resolve(p.ProductName, p.ProductImage); item.Category = category;
        }
        public void SyncPrices(MyStoreEntities db)
        {
            var cart = GetCart(); var ids = cart.Items.Select(x => x.ProductID).ToList();
            if (ids.Count == 0) return;
            var products = db.Products.Where(x => ids.Contains(x.ProductID)).ToList();
            foreach (var i in cart.Items)
            {
                var p = products.FirstOrDefault(x => x.ProductID == i.ProductID);
                if (p == null) { i.Selected = false; continue; }
                i.UnitPrice = p.ProductPrice; i.ProductName = p.ProductName; i.ProductImage = ProductImageHelper.Resolve(p.ProductName, p.ProductImage);
                i.Category = db.Categories.Where(x => x.CategoryID == p.CategoryID).Select(x => x.CategoryName).FirstOrDefault() ?? "Khác";
                if (i.Quantity > p.StockQuantity) i.Selected = false;
            }
        }
        public void Remove(int id) { GetCart().Items.RemoveAll(x => x.ProductID == id); }
        public void Clear() { GetCart().Items.Clear(); }
        public void RemovePurchased(IEnumerable<int> ids)
        {
            var set = new HashSet<int>(ids); GetCart().Items.RemoveAll(x => set.Contains(x.ProductID));
        }
    }

    // Anh mac dinh cho 20 san pham mau; khong can sua DB cu.
    public static class ProductImageHelper
    {
        private static readonly System.Collections.Generic.Dictionary<string, string> Images =
            new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "Sổ tay bìa cứng A5", "~/Content/images/products/so-tay-a5.jpg" },
            { "Bộ bút gel 6 màu", "~/Content/images/products/but-gel-6-mau.jpg" },
            { "Sổ kế hoạch tuần", "~/Content/images/products/so-ke-hoach-tuan.jpg" },
            { "Bút máy tối giản", "~/Content/images/products/but-may-toi-gian.jpg" },
            { "Chuột không dây", "~/Content/images/products/chuot-khong-day.jpg" },
            { "Bàn phím cơ compact", "~/Content/images/products/ban-phim-co-compact.jpg" },
            { "Tai nghe Bluetooth", "~/Content/images/products/tai-nghe-bluetooth.jpg" },
            { "Đèn bàn LED USB", "~/Content/images/products/den-ban-led-usb.jpg" },
            { "Áo thun basic", "~/Content/images/products/ao-thun-basic.jpg" },
            { "Áo khoác nhẹ", "~/Content/images/products/ao-khoac-nhe.jpg" },
            { "Túi tote canvas", "~/Content/images/products/tui-tote-canvas.jpg" },
            { "Mũ lưỡi trai", "~/Content/images/products/mu-luoi-trai.jpg" },
            { "Bình nước giữ nhiệt", "~/Content/images/products/binh-nuoc-giu-nhiet.jpg" },
            { "Ly sứ dáng tròn", "~/Content/images/products/ly-su-dang-tron.jpg" },
            { "Khay đựng đồ gỗ", "~/Content/images/products/khay-dung-do-go.jpg" },
            { "Hộp lưu trữ mini", "~/Content/images/products/hop-luu-tru-mini.jpg" },
            { "Ví đựng thẻ", "~/Content/images/products/vi-dung-the.jpg" },
            { "Dây đeo thẻ", "~/Content/images/products/day-deo-the.jpg" },
            { "Giá đỡ điện thoại", "~/Content/images/products/gia-do-dien-thoai.jpg" },
            { "Ốp lưng điện thoại", "~/Content/images/products/op-lung-dien-thoai.jpg" },
        };
        public static string Resolve(string name, string storedPath)
        {
            // 20 sample products have individual photographs included in the project.
            // Use those photos even for existing databases with empty / old sample paths.
            // Preserve any image uploaded by an admin to another path.
            var stored = string.IsNullOrWhiteSpace(storedPath) ? null : storedPath.Trim();
            string samplePath = null;
            var isSample = name != null && Images.TryGetValue(name.Trim(), out samplePath);
            var isOldSamplePath = stored != null &&
                stored.Replace('\\', '/').IndexOf("/Content/images/products/", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (isSample && (stored == null || isOldSamplePath))
                return samplePath + "?v=anh-rieng-20261117";
            return stored;
        }
    }
}
