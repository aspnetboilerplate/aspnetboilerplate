using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace MyAbpZeroProject.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : MyAbpZeroProjectControllerBase
    {
        public ActionResult Index()
        {
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }
	}
}