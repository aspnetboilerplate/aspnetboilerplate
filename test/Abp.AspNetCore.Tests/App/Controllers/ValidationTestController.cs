using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Runtime.Validation;
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

        [HttpPost]
        public JsonResult GetJsonValueWithEnum([FromBody] ValidationTestArgument2 arg2)
        {
            return Json(new ValidationTestArgument2
            {
                Value = arg2.Value
            });
        }

        public class ValidationTestArgument1
        {
            [Range(1, 99)]
            public int Value { get; set; }
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
}
