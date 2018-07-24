using System.Reflection;
using Abp.AspNetCore.App.MultiTenancy;
using Abp.AspNetCore.TestBase;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mocks;
using Abp.Auditing;
using Abp.FluentValidation;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Abp.AspNetCore.App
{
    [DependsOn(typeof(AbpAspNetCoreTestBaseModule), typeof(AbpFluentValidationModule))]
    public class AppModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            Configuration.ReplaceService<IAuditingStore, MockAuditingStore>();
            Configuration.ReplaceService<ITenantStore, TestTenantStore>();

            Configuration
                .Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AppModule).GetAssembly()
                );

            Configuration.IocManager.Resolve<IAbpAspNetCoreConfiguration>().RouteConfiguration.Add(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AppModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var localizationConfiguration = IocManager.IocContainer.Resolve<ILocalizationConfiguration>();
            localizationConfiguration.Languages.Add(new LanguageInfo("en-US", "English", isDefault: true));
            localizationConfiguration.Languages.Add(new LanguageInfo("it", "Italian"));
        }
    }
}
