using System.Reflection;
using Abp.Modules;

namespace Abp.Web.SignalR
{
    /// <summary>
    /// ABP SignalR integration module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpWebSignalRModule : AbpModule
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
