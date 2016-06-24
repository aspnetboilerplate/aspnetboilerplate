using Abp.AspNetCore.Mvc.Controllers;

namespace AbpAspNetCoreDemo.Controllers
{
    public abstract class MyDemoControllerBase : AbpController
    {
        protected MyDemoControllerBase()
        {
            LocalizationSourceName = "AbpAspNetCoreDemoModule";
        }
    }
}
