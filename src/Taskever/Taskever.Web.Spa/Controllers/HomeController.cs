using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
