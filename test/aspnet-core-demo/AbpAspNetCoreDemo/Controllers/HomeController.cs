using Abp.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class HomeController : DemoControllerBase
    {
        public IActionResult Index(string returnUrl = "")
        {
            var islocal = AbpUrlHelper.IsLocalUrl(Request, returnUrl);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = L("AboutDescription");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
