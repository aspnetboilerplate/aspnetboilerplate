using Abp.HtmlSanitizer.HtmlSanitizer;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers;

public class SanitizerTestController : DemoControllerBase
{
    [SanitizeHtml(KeepChildNodes = true)]
    [HttpPost("sanitizerTest/sanitizeHtml")]
    public string SanitizeHtml(string html)
    {
        return html;
    }
    
}