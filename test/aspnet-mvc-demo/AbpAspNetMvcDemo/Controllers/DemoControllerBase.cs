using Abp.IdentityFramework;
using Abp.Web.Mvc.Controllers;
using Microsoft.AspNet.Identity;

namespace AbpAspNetMvcDemo.Controllers
{
    public abstract class DemoControllerBase : AbpController
    {
        protected DemoControllerBase()
        {
            LocalizationSourceName = "AbpAspNetMvcDemo";
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}