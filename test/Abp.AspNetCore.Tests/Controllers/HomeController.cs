using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Tests.Controllers
{
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return Content("Hello world...");
        }
    }
}
