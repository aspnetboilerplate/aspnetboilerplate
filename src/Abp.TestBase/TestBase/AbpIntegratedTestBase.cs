using System;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    /// <summary>
    /// This is the base class for all tests integrated to ABP.
    /// </summary>
    public abstract class AbpIntegratedTestBase<TStartupModule> : IDisposable 
        where TStartupModule : AbpModule
    {
        /// <summary>
        /// Local <see cref="IIocManager"/> used for this test.
        /// </summary>
        protected IIocManager LocalIocManager { get; }

        protected AbpBootstrapper AbpBootstrapper { get; }

        /// <summary>
        /// Gets Session object. Can be used to change current user and tenant in tests.
        /// </summary>
        protected TestAbpSession AbpSession { get; private set; }

        protected AbpIntegratedTestBase(bool initializeAbp = true, IIocManager localIocManager = null)
        {
            LocalIocManager = localIocManager ?? new IocManager();

            AbpBootstrapper = AbpBootstrapper.Create<TStartupModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            if (initializeAbp)
            {
                InitializeAbp();
            }
        }

        protected void InitializeAbp()
        {
            LocalIocManager.RegisterIfNot<IAbpSession, TestAbpSession>();

            PreInitialize();

            AbpBootstrapper.Initialize();

            PostInitialize();

            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }

        /// <summary>
        /// This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PreInitialize()
        {

        }

        /// <summary>
        /// This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PostInitialize()
        {

        }

        public virtual void Dispose()
        {
            AbpBootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <typeparamref name="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <typeparamref name="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <paramref name="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <paramref name="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Registers given type if it's not registered before.
        /// </summary>
        /// <param name="type">Type to check and register</param>
        /// <param name="lifeStyle">Lifestyle</param>
        protected void EnsureClassRegistered(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                if (!type.GetTypeInfo().IsClass || type.GetTypeInfo().IsAbstract)
                {
                    throw new AbpException("Can not register " + type.Name + ". It should be a non-abstract class. If not, it should be registered before.");
                }

                LocalIocManager.Register(type, lifeStyle);
            }
        }

        protected virtual void WithUnitOfWork(Action action, UnitOfWorkOptions options = null)
        {
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions()))
                {
                    action();
                    uow.Complete();
                }
            }
        }

        protected virtual async Task WithUnitOfWorkAsync(Func<Task> action, UnitOfWorkOptions options = null)
        {
            using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(options ?? new UnitOfWorkOptions()))
                {
                    await action();
                    uow.Complete();
                }
            }
        }
    }
}
