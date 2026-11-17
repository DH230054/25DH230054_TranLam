using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace MyStore.Web.Models
{
    // Database FIRST: ánh xạ tới CSDL MyStore đã tạo bằng script SQL.
    // Không tự tạo / sửa schema runtime (Database.SetInitializer(null)).
    public class MyStoreEntities : DbContext
    {
        static MyStoreEntities() { Database.SetInitializer<MyStoreEntities>(null); }
        public MyStoreEntities() : base("name=MyStoreEntities") { }
        public DbSet<StoreUser> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreOrder> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(DbModelBuilder b)
        {
            b.Conventions.Remove<PluralizingTableNameConvention>();
            b.Entity<StoreUser>().Property(x => x.Username).IsUnicode(true);
            b.Entity<StoreUser>().Property(x => x.Password).IsUnicode(true);
            b.Entity<Product>().Property(x => x.ProductPrice).HasPrecision(18, 2);
            b.Entity<StoreOrder>().Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Entity<StoreOrder>().Property(x => x.ShippingFee).HasPrecision(18, 2);
            b.Entity<StoreOrder>().Property(x => x.OrderDate).HasColumnType("date");
            b.Entity<OrderDetail>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Entity<OrderDetail>().Property(x => x.TotalPrice).HasPrecision(18, 2);
            b.Entity<Customer>().HasRequired(x => x.User).WithMany().HasForeignKey(x => x.Username).WillCascadeOnDelete(false);
            b.Entity<Product>().HasRequired(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryID).WillCascadeOnDelete(false);
            b.Entity<StoreOrder>().HasRequired(x => x.Customer).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerID).WillCascadeOnDelete(false);
            b.Entity<OrderDetail>().HasRequired(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderID).WillCascadeOnDelete(false);
            b.Entity<OrderDetail>().HasRequired(x => x.Product).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductID).WillCascadeOnDelete(false);
            base.OnModelCreating(b);
        }
    }
}
