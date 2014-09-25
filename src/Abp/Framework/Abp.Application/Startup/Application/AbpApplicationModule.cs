using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup.Application
{
    /// <summary>
    /// This module is used to simplify and standardize building the "Application Layer" of an application.
    /// </summary>
    public class AbpApplicationModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext context)
        {
            base.PreInitialize(context);
            ApplicationLayerInterceptorRegisterer.Initialize(context);
        }

        public override void Initialize(IAbpInitializationContext context)
        {
            base.Initialize(context);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
