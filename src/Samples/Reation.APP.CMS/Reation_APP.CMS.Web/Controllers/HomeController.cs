using System.Web.Mvc;

namespace Reation_APP.CMS.Web.Controllers
{
    public class HomeController : CMSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}