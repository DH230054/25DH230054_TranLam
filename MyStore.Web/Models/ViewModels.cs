using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace MyStore.Web.Models
{
    public class RegisterVM
    {
        [Required, StringLength(80, MinimumLength=3), RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage="Chỉ dùng chữ không dấu, số và dấu gạch dưới."), Display(Name="Tên đăng nhập")]
        public string Username { get; set; }
        [Required, StringLength(80,MinimumLength=8), DataType(DataType.Password), Display(Name="Mật khẩu")] public string Password { get; set; }
        [Required, Compare("Password",ErrorMessage="Mật khẩu xác nhận không khớp."), DataType(DataType.Password), Display(Name="Xác nhận mật khẩu")] public string ConfirmPassword { get; set; }
        [Required, StringLength(120), Display(Name="Họ tên")] public string CustomerName { get; set; }
        [Required, RegularExpression(@"^\+?[0-9]{9,15}$"), Display(Name="Số điện thoại")] public string CustomerPhone { get; set; }
        [EmailAddress, Display(Name="Email")] public string CustomerEmail { get; set; }
        [StringLength(400), Display(Name="Địa chỉ")] public string CustomerAddress { get; set; }
    }
    public class LoginVM
    {
        [Required, Display(Name="Tên đăng nhập")] public string Username { get; set; }
        [Required, DataType(DataType.Password), Display(Name="Mật khẩu")] public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class ProfileVM
    {
        [Required, StringLength(120), Display(Name="Họ tên")] public string CustomerName { get; set; }
        [Required, RegularExpression(@"^\+?[0-9]{9,15}$"), Display(Name="Số điện thoại")] public string CustomerPhone { get; set; }
        [EmailAddress, Display(Name="Email")] public string CustomerEmail { get; set; }
        [StringLength(400), Display(Name="Địa chỉ")] public string CustomerAddress { get; set; }
    }
    public class ChangePasswordVM
    {
        [Required, DataType(DataType.Password), Display(Name="Mật khẩu hiện tại")] public string OldPassword { get; set; }
        [Required, StringLength(80, MinimumLength=8), DataType(DataType.Password), Display(Name="Mật khẩu mới")] public string NewPassword { get; set; }
        [Required, Compare("NewPassword"), DataType(DataType.Password), Display(Name="Nhập lại mật khẩu mới")] public string ConfirmPassword { get; set; }
    }
    public class AdminUserVM
    {
        [Required, StringLength(80,MinimumLength=3), RegularExpression(@"^[a-zA-Z0-9_]+$")] public string Username { get; set; }
        [Required, StringLength(80,MinimumLength=8), DataType(DataType.Password)] public string Password { get; set; }
    }
    public class ProductSearchVM
    {
        public string Keyword { get; set; }
        [Range(typeof(decimal),"0","999999999999")] public decimal? MinPrice { get; set; }
        [Range(typeof(decimal),"0","999999999999")] public decimal? MaxPrice { get; set; }
        public int? CategoryID { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalCount { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    public class HomeProductVM
    {
        public IList<Product> Featured { get; set; } = new List<Product>();
        public IList<Product> NewProducts { get; set; } = new List<Product>();
        public IList<Category> Categories { get; set; } = new List<Category>();
    }
    public class ProductDetailsVM
    {
        public Product Product { get; set; }
        public IList<Product> RelatedProducts { get; set; } = new List<Product>();
    }
    [Serializable]
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool Selected { get; set; } = true;
        public decimal TotalPrice => Quantity * UnitPrice;
    }
    [Serializable]
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total => Items.Where(x => x.Selected).Sum(x => x.TotalPrice);
        public int Count => Items.Sum(x => x.Quantity);
        public IEnumerable<IGrouping<string,CartItem>> GroupedItems => Items.GroupBy(x => x.Category);
        public IList<Product> SimilarProducts { get; set; } = new List<Product>();
    }
    public class CheckoutVM
    {
        [Required, StringLength(400, MinimumLength=6), Display(Name="Địa chỉ giao hàng")] public string ShippingAddress { get; set; }
        [Required, RegularExpression("^(COD|PayPal)$",ErrorMessage="Hình thức thanh toán không hợp lệ."), Display(Name="Thanh toán")] public string PaymentMethod { get; set; } = "COD";
        [Required, RegularExpression("^(Economy|Standard|Express)$",ErrorMessage="Hình thức giao hàng không hợp lệ."), Display(Name="Giao hàng")] public string DeliveryMethod { get; set; } = "Standard";
        public decimal TotalAmount { get; set; } // Tam tinh tien hang
        public decimal ShippingFee { get; set; } // Tinh lai o server
        public decimal GrandTotal => TotalAmount + ShippingFee;
        public IList<CartItem> Items { get; set; } = new List<CartItem>();
    }
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }
        public int CartCount { get; set; }
    }
    public class OrderHistoryVM
    {
        public string Status { get; set; }
        public string Keyword { get; set; }
        public string Expand { get; set; }
        public IList<OrderStatusGroupVM> Groups { get; set; } = new List<OrderStatusGroupVM>();
        public int TotalFound => Groups.Sum(x => x.TotalCount);
    }
    public class OrderStatusGroupVM
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public bool Expanded { get; set; }
        public int TotalCount { get; set; }
        public IList<StoreOrder> Orders { get; set; } = new List<StoreOrder>();
    }
}
