using Abp.HtmlSanitizer;
using Abp.Web.Models;
using AbpAspNetCoreDemo.Model;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers;

[ApiController]
[DontWrapResult]
[HtmlSanitizer]
public class SanitizerTestController : DemoControllerBase
{
    [HttpPost("sanitizerTest/sanitizeHtmlTest")]
    public MyModel HtmlSanitizer(MyModel myModel)
    {
        return myModel;
    }

    [HttpPost("sanitizerTest/sanitizeHtmlPropertyTest")]
    public object HtmlSanitizerWithProperty([FromForm] string firstInput, [FromForm] string secondInput)
    {
        return new
        {
            firstInput,
            secondInput
        };
    }

    [HttpPost("sanitizerTest/sanitizeInnerModelTest")]
    public MyModel SanitizeInnerModel(MyModel myModel)
    {
        return myModel;
    }

    [HttpPost("sanitizerTest/sanitizeAttributedPropertyModelTest")]
    public MyAttributedPropertyModel SanitizeAttributedPropertyModel(MyAttributedPropertyModel myModel)
    {
        return myModel;
    }

}