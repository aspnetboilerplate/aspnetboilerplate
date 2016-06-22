using System;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Session;
using NuGet.Packaging;

namespace Abp.AspNetCore.Tests.Infrastructure
{
    public static class AbpServiceOptionsTestExtensions
    {
        public static void SetupTest(this AbpServiceOptions options)
        {
            options.SetupTest(testOptions => { });
        }

        public static void SetupTest(this AbpServiceOptions options, Action<TestOptions> testOptionsAction)
        {
            var testOptions = new TestOptions();
            testOptions.Modules.Add<AbpAspNetCoreTestBaseModule>();

            testOptionsAction(testOptions);

            options.IocManager = new IocManager();

            options.IocManager.Register<IModuleFinder, TestModuleFinder>();
            options.IocManager.Register<IAbpSession, TestAbpSession>();
            
            var modules = options.IocManager.Resolve<TestModuleFinder>().Modules;
            modules.AddRange(testOptions.Modules);
        }
    }
}