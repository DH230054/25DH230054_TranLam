using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using(var db=new MyStoreEntities())
            {
                ViewBag.Products=db.Products.Count();ViewBag.Categories=db.Categories.Count();
                ViewBag.Customers=db.Customers.Count();ViewBag.Orders=db.Orders.Count();
                ViewBag.Revenue=db.Orders.Where(x=>x.OrderStatus!="Cancelled").Select(x=>(decimal?)x.TotalAmount).Sum()??0m;
                ViewBag.Pending=db.Orders.Count(x=>x.OrderStatus=="Pending");
                ViewBag.Processing=db.Orders.Count(x=>x.OrderStatus=="Processing");
                ViewBag.Shipping=db.Orders.Count(x=>x.OrderStatus=="Shipping");
                ViewBag.Delivered=db.Orders.Count(x=>x.OrderStatus=="Delivered");
                ViewBag.Cancelled=db.Orders.Count(x=>x.OrderStatus=="Cancelled");
                ViewBag.Months=db.Orders.Where(x=>x.OrderStatus!="Cancelled").GroupBy(x=>new{x.OrderDate.Year,x.OrderDate.Month})
                    .Select(g=>new {g.Key.Year,g.Key.Month,Amount=g.Sum(x=>x.TotalAmount)}).ToList()
                    .Select(x=>new ChartPoint{Label=x.Month+"/"+x.Year,Value=x.Amount}).ToList();
            }
            return View();
        }
        public class ChartPoint{public string Label{get;set;}public decimal Value{get;set;}}
    }
}
