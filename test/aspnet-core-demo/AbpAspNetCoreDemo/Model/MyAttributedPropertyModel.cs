using Abp.HtmlSanitizer;

namespace AbpAspNetCoreDemo.Model;

public class MyAttributedPropertyModel 
{
    [SanitizeHtml(KeepChildNodes = true)]
    public string KeepChildNodesInput { get; set; }
    
    public string DontSanitizeInput { get; set; }

    [SanitizeHtml(KeepChildNodes = false)]
    public string DontKeepChildNodesInput { get; set; }
}