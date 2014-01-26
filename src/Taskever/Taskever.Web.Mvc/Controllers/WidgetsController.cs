using System.Web.Mvc;

namespace Taskever.Web.Mvc.Controllers
{
    public class WidgetsController : Controller
    {
        public ActionResult LoginBox()
        {
            return PartialView("~/Widgets/LoginBox/LoginBox.cshtml");
        }
    }
}
