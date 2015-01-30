using System;
using Abp.Collections;
using Abp.Dependency;
using Abp.Modules;
using Abp.TestBase.Modules;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    public abstract class AbpIntegratedTest : IDisposable
    {
        protected IIocManager LocalIocManager { get; private set; }

        protected TestAbpSession AbpSession { get; private set; }

        private readonly AbpBootstrapper _bootstrapper;

        protected AbpIntegratedTest()
        {
            LocalIocManager = new IocManager();

            LocalIocManager.Register<IModuleFinder, TestModuleFinder>();

            AddModules(LocalIocManager.Resolve<TestModuleFinder>().Modules);

            _bootstrapper = new AbpBootstrapper(LocalIocManager);
            _bootstrapper.Initialize();

            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }

        public virtual void Dispose()
        {
            _bootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        protected virtual void AddModules(ITypeList<AbpModule> modules)
        {
            modules.Add<TestBaseModule>();
        }
    }
}
