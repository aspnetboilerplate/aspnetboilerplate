using Abp.Collections.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class TestController : DemoControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("api/test/getArray")]
        [HttpGet]
        public string TestGetArray(TestGetArrayModel model)
        {
            return model.Names.Length + " -> " + model.Names.JoinAsString(" # ");
        }
        
        [Route("api/person")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string CreatePerson()
        {
            return "42!";
        }
    }

    public class TestGetArrayModel
    {
        public string[] Names { get; set; }
    }
}