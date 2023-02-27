namespace AbpAspNetCoreDemo.Model;

public class MyInnerModel
{
    public string InnerHtmlInput { get; set; }
    
    public string InnerSecondInput  { get; set; }

    public MyDeepestInnerModel MyDeepestInnerModel { get; set; }
}