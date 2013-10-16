using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    [AbpAuthorize]
    public class HomeController : TaskeverController
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
