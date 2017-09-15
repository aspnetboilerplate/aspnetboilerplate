using System;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.ZeroCore.SampleApp;

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
        }

        public override void Initialize()
        {
            TestServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroTestModule).GetAssembly());
        }
    }
}
