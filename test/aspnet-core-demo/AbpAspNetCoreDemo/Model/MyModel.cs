using Abp.HtmlSanitizer;

namespace AbpAspNetCoreDemo.Model;

public class MyModel
{
    public string HtmlInput { get; set; }
    
    public string SecondInput { get; set; }

    public MyInnerModel MyInnerModel { get; set; }
}