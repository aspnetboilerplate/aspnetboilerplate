using System;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.TestBase.Modules;
using Abp.TestBase.Runtime.Session;

namespace Abp.AspNetCore.TestBase
{
    public static class AbpServiceOptionsTestExtensions
    {
        public static void SetupTest(this AbpServiceOptions options)
        {
            options.SetupTest(testOptions => { });
        }

        public static void SetupTest(this AbpServiceOptions options, Action<TestOptions> testOptionsAction)
        {
            options.IocManager = new IocManager();

            var testOptions = new TestOptions();
            testOptions.Modules.Add<AbpAspNetCoreTestBaseModule>();

            testOptionsAction(testOptions);

            options.IocManager.RegisterIfNot<IModuleFinder, TestModuleFinder>();
            options.IocManager.RegisterIfNot<IAbpSession, TestAbpSession>();
            
            var modules = options.IocManager.Resolve<TestModuleFinder>().Modules;
            foreach (var module in testOptions.Modules)
            {
                modules.Add(module);
            }
        }
    }
}