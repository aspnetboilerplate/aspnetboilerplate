using System;
using Abp.Collections;
using Abp.Dependency;
using Abp.Modules;
using Abp.TestBase.Modules;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    /// <summary>
    /// This is the base class for all tests integrated to ABP.
    /// </summary>
    public abstract class AbpIntegratedTestBase : IDisposable
    {
        protected IIocManager LocalIocManager { get; private set; }

        protected TestAbpSession AbpSession { get; private set; }

        private readonly AbpBootstrapper _bootstrapper;

        protected AbpIntegratedTestBase()
        {
            LocalIocManager = new IocManager();

            LocalIocManager.Register<IModuleFinder, TestModuleFinder>();

            AddModules(LocalIocManager.Resolve<TestModuleFinder>().Modules);

            PreInitialize();

            _bootstrapper = new AbpBootstrapper(LocalIocManager);
            _bootstrapper.Initialize();

            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }

        protected virtual void PreInitialize()
        {
            //This method can be overrided to replace some services with fakes.
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
