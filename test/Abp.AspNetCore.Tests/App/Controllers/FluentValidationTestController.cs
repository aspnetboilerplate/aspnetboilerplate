using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class FluentValidationTestController : AbpController
    {
        #region Actions

        public JsonResult GetJsonValue([FromQuery] ValidationTestArgument1 arg1)
        {
            return Json(new ValidationTestArgument1
            {
                Value = arg1.Value
            });
        }

        public JsonResult GetJsonValueWithArray([FromBody] ValidationTestArgument2 arg1)
        {
            return Json(new ValidationTestArgument2
            {
                Array = arg1.Array
            });
        }

        public JsonResult GetNullableJsonValue([FromQuery] ValidationTestArgument3 arg1)
        {
            return Json(new ValidationTestArgument3
            {
                Value = arg1.Value
            });
        }

        public JsonResult GetNullableJsonValue2([FromBody]ValidationTestArgument3 arg1)
        {
            return Json(new ValidationTestArgument3
            {
                Value = arg1.Value
            });
        }

        #endregion

        #region Arguments

        public class ValidationTestArgument1
        {
            public int Value { get; set; }
        }

        public class ValidationTestArgument2
        {
            public ArrayItem[] Array { get; set; }
        }

        public class ArrayItem
        {
            public int Value { get; set; }
        }

        public class ValidationTestArgument3
        {
            public int? Value { get; set; }
        }

        #endregion

        #region Validators

        public class ValidationTestArgument1Validator : AbstractValidator<ValidationTestArgument1>
        {
            public ValidationTestArgument1Validator()
            {
                RuleFor(x => x.Value).InclusiveBetween(1, 99);
            }
        }

        public class ValidationTestArgument2Validator : AbstractValidator<ValidationTestArgument2>
        {
            public ValidationTestArgument2Validator()
            {
                RuleFor(x => x.Array).Must(ContainAtLeastThreeItems).WithMessage("Array must contain at least three items");
            }

            private bool ContainAtLeastThreeItems(ArrayItem[] array)
            {
                return array != null && array.Length >= 3;
            }
        }

        public class ArrayItemValidator : AbstractValidator<ArrayItem>
        {
            public ArrayItemValidator()
            {
                RuleFor(x => x.Value).GreaterThan(0);
            }
        }

        #endregion
    }
}
