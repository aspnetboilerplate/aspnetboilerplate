using System;
using Abp.Collections;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Session;
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
            LocalIocManager.Register<IAbpSession, TestAbpSession>();

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

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            return LocalIocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            return LocalIocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }
    }
}
