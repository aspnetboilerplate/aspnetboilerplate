using Abp.Application.Services;
using Abp.Application.Services.Interceptors;
using Abp.Modules;
using Castle.Core;

namespace Abp.Application.Startup
{
    [AbpModule("Abp.Application")]
    public class AbpApplicationModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpApplicationDependencyInstaller());
        }

        private void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if(typeof(IApplicationService).IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpApplicationServiceInterceptor)));
            }
        }
    }
}
