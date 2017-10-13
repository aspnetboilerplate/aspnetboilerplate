using Abp.AutoMapper;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.EntityFramework;
using Abp.ZeroCore.SampleApp.EntityFramework.Seed;

namespace Abp.ZeroCore.SampleApp
{
    [DependsOn(typeof(AbpZeroCoreEntityFrameworkCoreModule), typeof(AbpAutoMapperModule))]
    public class AbpZeroCoreSampleAppModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<SampleAppDbContext>(configuration =>
                {
                    AbpZeroTemplateDbContextConfigurer.Configure(configuration.DbContextOptions, configuration.ConnectionString);
                });
            }

            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            Configuration.Features.Providers.Add<AppFeatureProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreSampleAppModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
