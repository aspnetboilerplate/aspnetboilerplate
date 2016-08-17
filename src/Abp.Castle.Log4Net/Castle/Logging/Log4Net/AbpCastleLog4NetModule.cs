using System.Reflection;
using Abp.Modules;

namespace Abp.Castle.Logging.Log4Net
{
    /// <summary>
    /// ABP Castle Log4Net module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpCastleLog4NetModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}