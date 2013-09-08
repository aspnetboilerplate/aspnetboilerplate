using System.Web.Mvc;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
