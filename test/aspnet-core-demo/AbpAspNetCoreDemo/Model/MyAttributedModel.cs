using Abp.HtmlSanitizer.HtmlSanitizer;

namespace AbpAspNetCoreDemo.Model;

[SanitizeHtml]
public class MyAttributedModel 
{
    public string HtmlInput { get; set; }

    [SanitizeHtml(IsDisabled = true)]
    public string DontSanitizeInput { get; set; }

    [SanitizeHtml(KeepChildNodes = true)]
    public string KeepChildNodesInput { get; set; }
}