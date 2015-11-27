using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace ModuleZeroSampleProject.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : ModuleZeroSampleProjectControllerBase
    {
        public ActionResult Index()
        {
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }
    }
}