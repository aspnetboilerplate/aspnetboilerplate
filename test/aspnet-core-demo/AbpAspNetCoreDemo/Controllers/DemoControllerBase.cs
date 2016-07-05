using Abp.AspNetCore.Mvc.Controllers;

namespace AbpAspNetCoreDemo.Controllers
{
    public class DemoControllerBase : AbpController
    {
        public DemoControllerBase()
        {
            LocalizationSourceName = "AbpAspNetCoreDemoModule";
        }
    }
}
