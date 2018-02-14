using System.Collections.Generic;
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

        [HttpPost]
        public JsonResult GetJsonValueWithValidatableObject([FromBody] ValidationTestArgument3 arg3)
        {
            return Json(new ValidationTestArgument3
            {
                Value = arg3.Value
            });
        }

        [HttpPost]
        public JsonResult GetJsonValueWithCustomValidate([FromBody] ValidationTestArgument4 arg4)
        {
            return Json(new ValidationTestArgument4
            {
                Value = arg4.Value
            });
        }


        [HttpPost]
        public JsonResult GetJsonValueWithCombinedValidators([FromBody] ValidationTestArgument5 arg5)
        {
            return Json(new ValidationTestArgument5
            {
                Value = arg5.Value
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

        public class ValidationTestArgument3 : IValidatableObject
        {
            public int Value { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var results = new List<ValidationResult>();

                if (Value <= 0)
                {
                    results.Add(new ValidationResult("Value must be higher than 0", new [] {"value"}));
                }

                return results;
            }
        }

        public class ValidationTestArgument4 : ICustomValidate
        {
            public string Value { get; set; }

            public void AddValidationErrors(CustomValidationContext context)
            {
                if (Value != "abp")
                {
                    context.Results.Add(new ValidationResult("Value must be \"abp\"", new [] {"value"}));
                }
            }
        }

        public class ValidationTestArgument5 : ICustomValidate
        {
            [Range(1, 99)]
            public int Value { get; set; }

            public void AddValidationErrors(CustomValidationContext context)
            {
                if (Value % 2 != 0)
                {
                    context.Results.Add(new ValidationResult("Value must be an even number", new[] { "value" }));
                }
            }
        }
    }
}
