using Abp.Application.Services;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class DemoAppServiceBase : ApplicationService
    {
        public DemoAppServiceBase()
        {
            LocalizationSourceName = "AbpAspNetCoreDemoModule";
        }
    }
}
