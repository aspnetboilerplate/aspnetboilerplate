using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class TestController : DemoControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}