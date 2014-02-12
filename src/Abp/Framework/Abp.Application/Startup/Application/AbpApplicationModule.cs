using System.Reflection;
using Abp.Application.Authorization.Permissions;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup.Application
{
    /// <summary>
    /// This module is used to simplify and standardize building the "Application Layer" of an application.
    /// </summary>
    [AbpModule("Abp.Application")]
    public class AbpApplicationModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            ApplicationLayerInterceptorRegisterer.Initialize(initializationContext);
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
