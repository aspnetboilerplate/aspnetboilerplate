using System.Reflection;
using Abp.Modules;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.Authorization;
using Abp.Zero.SampleApp.Configuration;
using Abp.Zero.SampleApp.Features;

namespace Abp.Zero.SampleApp
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class SampleAppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Features.Providers.Add<AppFeatureProvider>();

            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            Configuration.MultiTenancy.IsEnabled = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
