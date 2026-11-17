using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MyStore.Web.Models
{
    [Table("User")]
    public class StoreUser
    {
        [Key, StringLength(255), Required]
        public string Username { get; set; }
        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(1)]
        public string UserRole { get; set; }
    }

    [Table("Customer")]
    public class Customer
    {
        [Key] public int CustomerID { get; set; }
        [Required, Display(Name = "Họ và tên"), StringLength(120)] public string CustomerName { get; set; }
        [Required, RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Số điện thoại phải có 9–15 chữ số."), Display(Name="Số điện thoại")]
        public string CustomerPhone { get; set; }
        [EmailAddress, Display(Name="Email")] public string CustomerEmail { get; set; }
        [Display(Name="Địa chỉ")] public string CustomerAddress { get; set; }
        [Required, StringLength(255)] public string Username { get; set; }
        [ForeignKey("Username")] public virtual StoreUser User { get; set; }
        public virtual ICollection<StoreOrder> Orders { get; set; } = new HashSet<StoreOrder>();
    }

    [Table("Category")]
    public class Category
    {
        [Key] public int CategoryID { get; set; }
        [Required, StringLength(120), Display(Name="Tên danh mục")] public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }

    [Table("Product")]
    public class Product
    {
        [Key] public int ProductID { get; set; }
        [Required, Display(Name="Danh mục")] public int CategoryID { get; set; }
        [Required, StringLength(160), Display(Name="Tên sản phẩm")] public string ProductName { get; set; }
        // Database2024.txt có lỗi chính tả ProductDecription. Ánh xạ giữ nguyên cột cũ.
        [Column("ProductDecription"), Required, Display(Name="Mô tả sản phẩm")]
        public string ProductDescription { get; set; }
        [Range(typeof(decimal), "0", "999999999999", ErrorMessage="Giá phải không âm."), Display(Name="Giá (VNĐ)")]
        public decimal ProductPrice { get; set; }
        [Display(Name="Đường dẫn ảnh")] public string ProductImage { get; set; }
        [Range(0, 1000000), Display(Name="Tồn kho")] public int StockQuantity { get; set; }
        [NotMapped, Display(Name="Tải ảnh lên")] public HttpPostedFileBase UploadImg { get; set; }
        [ForeignKey("CategoryID")] public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    [Table("Order")]
    public class StoreOrder
    {
        [Key] public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        // Phi giao hang luu rieng; TotalAmount = tong gia san pham + ShippingFee.
        public decimal ShippingFee { get; set; }
        [StringLength(30)] public string PaymentStatus { get; set; }
        [Required, Column("ShippingAddress"), StringLength(400)] public string ShippingAddress { get; set; }
        [Required, StringLength(30)] public string PaymentMethod { get; set; }
        [Required, StringLength(30)] public string DeliveryMethod { get; set; }
        [Required, StringLength(30)] public string OrderStatus { get; set; }
        [ForeignKey("CustomerID")] public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key, Column("ID")] public int ID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        [Range(1, 1000)] public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // Computed persisted column trong SQL: Quantity * UnitPrice.
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] public decimal TotalPrice { get; private set; }
        [ForeignKey("OrderID")] public virtual StoreOrder Order { get; set; }
        [ForeignKey("ProductID")] public virtual Product Product { get; set; }
    }
}
