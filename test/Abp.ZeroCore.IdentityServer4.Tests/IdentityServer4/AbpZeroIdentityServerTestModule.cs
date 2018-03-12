using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.EntityFramework;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.IdentityServer4
{
    [DependsOn(typeof(AbpZeroCoreIdentityServerEntityFrameworkCoreModule), typeof(AbpZeroTestModule))]
    public class AbpZeroIdentityServerTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            var services = new ServiceCollection();

            services.AddAbpIdentity<Tenant, User, Role>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAbpPersistedGrants<SampleAppDbContext>()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddAbpIdentityServer<User>()
                .AddProfileService<AbpProfileService<User>>();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroIdentityServerTestModule).GetAssembly());
        }
    }
}
