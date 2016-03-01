using Adorable.Modules;

namespace Adorable.Owin
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
