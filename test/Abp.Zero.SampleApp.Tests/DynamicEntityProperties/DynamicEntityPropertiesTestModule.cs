using Abp.Modules;
using Abp.Threading;
using Castle.MicroKernel.Registration;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    [DependsOn(typeof(SampleAppTestModule))]
    public class DynamicEntityPropertiesTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DynamicEntityPropertiesTestModule).Assembly);

            IocManager.IocContainer.Register(
                Component.For<ICancellationTokenProvider>().Instance(NullCancellationTokenProvider.Instance)
                    .LifestyleSingleton()
            );

            Configuration.Authorization.Providers.Add<DynamicEntityPropertiesTestAuthorizationProvider>();
            Configuration.DynamicEntityProperties.Providers.Add<MyDynamicEntityPropertyDefinitionProvider>();
        }
    }
}