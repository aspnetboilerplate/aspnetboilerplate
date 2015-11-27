using Abp.Web.Mvc.Controllers;

namespace ModuleZeroSampleProject.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class ModuleZeroSampleProjectControllerBase : AbpController
    {
        protected ModuleZeroSampleProjectControllerBase()
        {
            LocalizationSourceName = ModuleZeroSampleProjectConsts.LocalizationSourceName;
        }
    }
}