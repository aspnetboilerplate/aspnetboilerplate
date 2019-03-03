using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.Notifications;
using Abp.ZeroCore.SampleApp;
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
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.UnitOfWork.IsTransactional = false;
            Configuration.Notifications.Distributers.Add<FakeNotificationDistributer>();
            Configuration.Notifications.Notifiers.Add<FakeRealTimeNotifier1>();
            Configuration.Notifications.Notifiers.Add<FakeRealTimeNotifier2>();
        }

        public override void Initialize()
        {
            TestServiceCollectionRegistrar.Register(IocManager);

            IocManager.IocContainer.Register(
                Component
                    .For<FakeNotificationDistributer>()
                    .LifestyleSingleton()
            );

            IocManager.IocContainer.Register(
                Component
                    .For<FakeRealTimeNotifier1>()
                    .LifestyleSingleton()
            );

            IocManager.IocContainer.Register(
                Component
                    .For<FakeRealTimeNotifier2>()
                    .LifestyleSingleton()
            );

            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroTestModule).GetAssembly());
        }
    }
}
