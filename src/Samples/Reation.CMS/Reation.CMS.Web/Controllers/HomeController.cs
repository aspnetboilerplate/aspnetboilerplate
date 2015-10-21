using System.Web.Mvc;

namespace Reation.CMS.Web.Controllers
{
    public class HomeController : CMSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}