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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Redis;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using StackExchange.Redis;
using Xunit;


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
