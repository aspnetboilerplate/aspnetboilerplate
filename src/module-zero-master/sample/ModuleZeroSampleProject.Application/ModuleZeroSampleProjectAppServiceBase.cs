using Abp.Application.Services;

namespace ModuleZeroSampleProject
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ModuleZeroSampleProjectAppServiceBase : ApplicationService
    {
        protected ModuleZeroSampleProjectAppServiceBase()
        {
            LocalizationSourceName = ModuleZeroSampleProjectConsts.LocalizationSourceName;
        }
    }
}