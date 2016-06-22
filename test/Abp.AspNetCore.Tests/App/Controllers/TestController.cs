using Abp.AspNetCore.Mvc.Controllers;
using Abp.AspNetCore.Tests.App.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Tests.App.Controllers
{
    public class TestController : AbpController
    {
        public ActionResult SimpleContent()
        {
            return Content("Hello world...");
        }

        public JsonResult SimpleJson()
        {
            return Json(new SimpleViewModel("Forty Two", 42));
        }
    }
}
