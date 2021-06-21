using Abp.AutoMapper;
using Abp.Modules;
using Abp.Notifications;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.Notifications;
using Abp.ZeroCore.SampleApp;
using Abp.Configuration.Startup;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Registration;

namespace Abp.Zero
{
    [DependsOn(typeof(AbpZeroCoreSampleAppModule), typeof(AbpTestBaseModule))]
    public class AbpZeroTestModule : AbpModule
    {
        public AbpZeroTestModule(AbpZeroCoreSampleAppModule sampleAppModule)
        {
            sampleAppModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper is removed
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper is removed
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.UnitOfWork.IsTransactional = false;

            Configuration.ReplaceService<INotificationDistributer, FakeNotificationDistributer>();
        }

        public override void Initialize()
        {
            TestServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroTestModule).GetAssembly());
            IocManager.IocContainer.Register(
                Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>()
            );
        }
    }
}
