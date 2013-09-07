using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Abp.Modules;
using Abp.Services;
using Abp.Web.Controllers;
using Abp.Web.Controllers.Dynamic;
using Abp.Web.Dependency;
using Abp.Web.Dependency.Installers;
using Abp.Web.Dependency.Interceptors;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Abp.Web.Startup
{
    public class AbpBootstrapper : IDisposable
    {
        protected WindsorContainer IocContainer { get; private set; }

        public AbpBootstrapper()
        {
            IocContainer = new WindsorContainer();
            IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
            DynamicControllerGenerator.IocContainer = IocContainer;
            AbpModuleManager.Instance.IocContainer = IocContainer;
        }

        public void Initialize()
        {
            var initializationContext = new AbpInitializationContext(IocContainer);
            AbpModuleManager.Instance.PreInitializeModules(initializationContext);
            InitializeCore();
            AbpModuleManager.Instance.InitializeModules(initializationContext);
            AbpModuleManager.Instance.PostInitializeModules(initializationContext);
        }

        public void InitializeCore()
        {
            RouteConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocContainer.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(IocContainer));

            RegisterInstallers();
        }
        
        protected virtual void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (handler.ComponentModel.Implementation.IsSubclassOf(typeof(ApiController))) //TODO: Is that right?
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpApiControllerInterceptor)));
            }
        }

        protected virtual void RegisterInstallers()
        {
            IocContainer.Install(new AbpInstaller());
        }

        public void Dispose()
        {
            if (IocContainer != null)
            {
                IocContainer.Dispose();
            }
        }
    }
}
