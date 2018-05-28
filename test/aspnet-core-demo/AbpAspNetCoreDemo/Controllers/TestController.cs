using System.ComponentModel.DataAnnotations;
using Abp.Collections.Extensions;
using Abp.Web.Mvc.Alerts;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class TestController : DemoControllerBase
    {
        private readonly IAlertManager _alertManager;

        public TestController(IAlertManager alertManager)
        {
            _alertManager = alertManager;
        }

        public IActionResult Index()
        {
            Alerts.Info("Test alert message!", "Test Alert");
            return View(_alertManager.Alerts);
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

        [HttpPost]
        public JsonResult GetJsonValueWithEnum([FromBody] ValidationTestArgument2 arg2)
        {
            return Json(new ValidationTestArgument2
            {
                Value = arg2.Value
            });
        }
    }

    public class TestGetArrayModel
    {
        public string[] Names { get; set; }
    }

    public class ValidationTestArgument2
    {
        [EnumDataType(typeof(ValidationTestEnum))]
        public ValidationTestEnum Value { get; set; }
    }

    public enum ValidationTestEnum
    {
        Value1 = 1,
        Value2 = 2
    }
}