using System.Reflection;
using Abp.Modules;

namespace Abp.AspNetCore.SignalR
{
    /// <summary>
    /// ABP ASP.NET Core SignalR integration module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAspNetCoreSignalRModule : AbpModule
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
