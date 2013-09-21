using System.Web.Mvc;

namespace Taskever.Web.Areas.Abp.Modules.Core
{
    public class AbpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Abp/Modules/Core";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Abp_default",
                "Abp/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
