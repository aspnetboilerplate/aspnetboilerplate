using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Microsoft.AspNet.Identity;

namespace Taskever.Web.Mvc.Controllers
{
    [AbpAuthorize]
    public class HomeController : TaskeverController
    {
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            return View("Index");
        }
    }
}
