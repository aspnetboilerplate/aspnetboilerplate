using System.Web.Mvc;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    [Authorize]
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            var user = System.Web.HttpContext.Current.User;
            return View();
        }
    }
}
