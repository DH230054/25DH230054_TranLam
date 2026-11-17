using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Web.Models;
using MyStore.Web.Services;
namespace MyStore.Web.Areas.Admin.Controllers
{
    [AdminOnly]
    public class CustomersController : Controller
    {
        readonly MyStoreEntities db=new MyStoreEntities();
        public ActionResult Index(string keyword,string sortOrder,int page=1)
        {
            if(page<1)return new HttpStatusCodeResult(400);
            var q=db.Customers.AsQueryable();if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.CustomerName.Contains(keyword)||x.Username.Contains(keyword)||x.CustomerPhone.Contains(keyword));
            ViewBag.Keyword=keyword;ViewBag.SortOrder=sortOrder;ViewBag.Page=page;ViewBag.Total=q.Count();
            q=sortOrder=="desc"?q.OrderByDescending(x=>x.CustomerName):q.OrderBy(x=>x.CustomerName);
            return View(q.Skip((page-1)*10).Take(10).ToList());
        }
        public ActionResult Details(int id)
        {
            var c=db.Customers.Include(x=>x.Orders).FirstOrDefault(x=>x.CustomerID==id);
            return c==null?(ActionResult)HttpNotFound():View(c);
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
