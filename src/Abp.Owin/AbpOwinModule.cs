using Abp.Modules;

namespace Abp.Owin
{
    /// <summary>
    /// OWIN integration module for ABP.
    /// </summary>
    [DependsOn(typeof (AbpKernelModule))]
    public class AbpOwinModule : AbpModule
    {
        //nothing to do...
    }
}
