using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : AbpController
    {
        //[AbpMvcAuthorize(Features = "TestFeature")]
        public ActionResult Index()
        {
            var user = System.Web.HttpContext.Current.User;
            return View();
        }
    }
}
