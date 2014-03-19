using Abp.Web.Mvc.Controllers;

namespace MySpaProject.Web.Controllers
{
    public abstract class MySpaProjectControllerBase : AbpController
    {
        protected MySpaProjectControllerBase()
        {
            LocalizationSourceName = "MySpaProject";
        }
    }
}