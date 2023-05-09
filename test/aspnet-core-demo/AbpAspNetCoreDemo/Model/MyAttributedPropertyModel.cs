using Abp.HtmlSanitizer;

namespace AbpAspNetCoreDemo.Model;

public class MyAttributedPropertyModel 
{
    public string KeepChildNodesInput { get; set; }
    
    [DisableHtmlSanitizer]
    public string DontSanitizeInput { get; set; }

    public string DontKeepChildNodesInput { get; set; }
}