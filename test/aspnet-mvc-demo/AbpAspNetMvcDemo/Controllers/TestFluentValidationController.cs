using System.Web.Mvc;
using FluentValidation;

namespace AbpAspNetMvcDemo.Controllers
{
    public class TestFluentValidationController : DemoControllerBase
    {
        public ContentResult GetJsonValue(MyCustomArgument arg1)
        {
            return Content(arg1.Value.ToString());
        }

        public class MyCustomArgument
        {
            public int Value { get; set; }
        }

        public class ValidationTestArgument1Validator : AbstractValidator<MyCustomArgument>
        {
            public ValidationTestArgument1Validator()
            {
                RuleFor(x => x.Value).InclusiveBetween(1, 99);
            }
        }
    }
}