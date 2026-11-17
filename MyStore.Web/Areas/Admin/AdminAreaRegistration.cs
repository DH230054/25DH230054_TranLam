using System.Web.Mvc;
namespace MyStore.Web.Areas.Admin
{
    public class AdminAreaRegistration:AreaRegistration
    {
        public override string AreaName => "Admin";
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("Admin_default","Admin/{controller}/{action}/{id}",
                new{controller="Home",action="Index",id=UrlParameter.Optional},
                new[]{"MyStore.Web.Areas.Admin.Controllers"});
        }
    }
}
