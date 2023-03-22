using Abp.HtmlSanitizer;
using Abp.Web.Models;
using AbpAspNetCoreDemo.Model;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers;

[ApiController]
[DontWrapResult]
public class SanitizerTestController : DemoControllerBase
{
    [SanitizeHtml]
    [HttpPost("sanitizerTest/sanitizeHtmlTest")]
    public MyModel SanitizeHtml(MyModel myModel)
    {
        return myModel;
    }

    [SanitizeHtml]
    [HttpPost("sanitizerTest/sanitizeHtmlPropertyTest")]
    public object SanitizeHtmlWithProperty([FromForm] string firstInput, [FromForm] string secondInput)
    {
        return new
        {
            firstInput, secondInput
        };
    }

    [SanitizeHtml(KeepChildNodes = true)]
    [HttpPost("sanitizerTest/sanitizeHtmlKeepChildNodesTest")]
    public MyModel SanitizeHtmlKeepChildNodes(MyModel myModel)
    {
        return myModel;
    }
    
    [SanitizeHtml]
    [HttpPost("sanitizerTest/sanitizeInnerModelTest")]
    public MyModel SanitizeInnerModel(MyModel myModel)
    {
        return myModel;
    }
    
    [HttpPost("sanitizerTest/sanitizeAttributedModelTest")]
    public MyAttributedModel SanitizeAttributedModel([SanitizeHtml(IsDisabled = true)]MyAttributedModel myModel)
    {
        return myModel;
    }
    
    [HttpPost("sanitizerTest/sanitizeAttributedPropertyModelTest")]
    public MyAttributedPropertyModel SanitizeAttributedPropertyModel(MyAttributedPropertyModel myModel)
    {
        return myModel;
    }
}