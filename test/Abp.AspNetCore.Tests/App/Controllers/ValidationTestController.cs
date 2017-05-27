using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class ValidationTestController : AbpController
    {
        public ActionResult GetContentValue([FromQuery] ValidationTestArgument1 arg1)
        {
            return Content("OK: " + arg1.Value);
        }

        public JsonResult GetJsonValue([FromQuery] ValidationTestArgument1 arg1)
        {
            return Json(new ValidationTestArgument1
            {
                Value = arg1.Value
            });
        }

        public class ValidationTestArgument1
        {
            [Range(1, 99)]
            public int Value { get; set; }
        }
    }
}
