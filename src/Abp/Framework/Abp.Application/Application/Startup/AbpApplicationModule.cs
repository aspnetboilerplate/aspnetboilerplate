using Abp.Application.Services.Dto.Validation;
using Abp.Modules;
using Abp.Startup;

namespace Abp.Application.Startup
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
            initializationContext.IocContainer.Install(new ValidationInstaller());
        }
    }
}
