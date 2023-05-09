using Abp.HtmlSanitizer;

namespace AbpAspNetCoreDemo.Model;

public class MyAttributedModel 
{
    public string HtmlInput { get; set; }

    [DisableHtmlSanitizer]
    public string DontSanitizeInput { get; set; }

    public string KeepChildNodesInput { get; set; }
}