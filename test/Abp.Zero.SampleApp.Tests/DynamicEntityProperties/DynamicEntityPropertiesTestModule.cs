using Abp.Modules;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    [DependsOn(typeof(SampleAppTestModule))]
    public class DynamicEntityPropertiesTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DynamicEntityPropertiesTestModule).Assembly);

            Configuration.Authorization.Providers.Add<DynamicEntityPropertiesTestAuthorizationProvider>();
            Configuration.DynamicEntityProperties.Providers.Add<MyDynamicEntityPropertyDefinitionProvider>();
        }
    }
}