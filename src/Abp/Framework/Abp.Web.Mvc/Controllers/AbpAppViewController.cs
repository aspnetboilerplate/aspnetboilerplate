using System.Web.Mvc;

namespace Abp.Web.Mvc.Controllers
{
    //TODO: Maybe it's better to write an HTTP handler for that instead of controller (since it's more light)
    public class AbpAppViewController : AbpController
    {
        public ActionResult Load(string viewUrl)
        {
            return View(viewUrl);
        }
    }
}
