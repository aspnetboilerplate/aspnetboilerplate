using System.Web.Mvc;

namespace Abp.Modules.Core.Mvc.Controllers
{
    //TODO: Maybe it's better to write an HTTP handler for that instead of controller (since it's more light)
    public class DurandalViewController : Controller
    {
        public ActionResult GetAppView(string viewUrl)
        {
            if (viewUrl.StartsWith("/"))
            {
                return View(viewUrl);
            }

            return View("/App/Main/" + viewUrl);
        }
    }
}
