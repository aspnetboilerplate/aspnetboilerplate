using Abp.AutoMapper;
using Abp.Modules;
using Abp.Notifications;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.Notifications;
using Abp.ZeroCore.SampleApp;
using Abp.Configuration.Startup;
using Abp.Runtime.Caching.Redis;

namespace Abp.Zero
{
    [DependsOn(typeof(AbpZeroCoreSampleAppModule), typeof(AbpTestBaseModule), typeof(AbpAspNetCorePerRequestRedisCacheModule))]
    public class AbpZeroTestModule : AbpModule
    {
        public AbpZeroTestModule(AbpZeroCoreSampleAppModule sampleAppModule)
        {
            sampleAppModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.UnitOfWork.IsTransactional = false;

            Configuration.ReplaceService<INotificationDistributer, FakeNotificationDistributer>();
        }

        public override void Initialize()
        {
            TestServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroTestModule).GetAssembly());
        }
    }
}