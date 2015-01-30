using System;
using System.Reflection;
using Abp.Application.Services.Interceptors;
using Abp.Authorization.Interceptors;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;
using Abp.TestBase.Runtime.Session;

namespace Abp.TestBase
{
    public abstract class AbpIntegratedTest : IDisposable
    {
        public IIocManager LocalIocManager { get; private set; }

        protected TestAbpSession AbpSession { get; private set; }

        protected AbpIntegratedTest()
        {
            LocalIocManager = new IocManager();

            PreInitialize();
            Initialize();
            PostInitialize();
        }

        public virtual void Dispose()
        {
            Shutdown();
            LocalIocManager.Dispose();
        }

        protected virtual void PreInitialize()
        {
            LocalIocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            ApplicationServiceInterceptorRegistrar.Initialize(LocalIocManager);
        }

        protected virtual void Initialize()
        {
            LocalIocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly()); //TODO: Maybe registering entire assembly is not good..?
            LocalIocManager.Register<ValidationInterceptor>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<AuthorizationInterceptor>(DependencyLifeStyle.Transient);
        }

        protected virtual void PostInitialize()
        {
            AbpSession = LocalIocManager.Resolve<TestAbpSession>();
        }

        protected virtual void Shutdown()
        {
            
        }
    }
}
