using System.Reflection;
using Abp.Modules;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR
{
    /// <summary>
    /// ABP SignalR integration module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpWebSignalRModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            GlobalHost.DependencyResolver = new WindsorDependencyResolver(IocManager.IocContainer);
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
