using Abp.Modules;

namespace Abp.Castle.Logging.NLog
{
    /// <summary>
    /// ABP Castle NLog module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpCastleNLogModule : AbpModule
    {

    }
}
