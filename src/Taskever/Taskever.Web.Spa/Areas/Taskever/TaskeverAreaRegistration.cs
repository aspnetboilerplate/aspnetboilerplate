using System.Web.Mvc;

namespace Taskever.Web.Areas.Taskever
{
    public class TaskeverAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Taskever";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Taskever_default",
                "Taskever/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
