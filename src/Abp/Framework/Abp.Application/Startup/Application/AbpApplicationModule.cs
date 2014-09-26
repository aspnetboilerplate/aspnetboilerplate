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
        public override void PreInitialize()
        {
            base.PreInitialize();
            ApplicationLayerInterceptorRegisterer.Initialize(IocManager);
        }

        public override void Initialize()
        {
            base.Initialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
