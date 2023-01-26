using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers;

public class SanitizerTestController : DemoControllerBase
{
    [HttpPost("sanitizerTest/sanitizeHtml")]
    public string SanitizeHtml(string html)
    {
        return html;
    }
    
}