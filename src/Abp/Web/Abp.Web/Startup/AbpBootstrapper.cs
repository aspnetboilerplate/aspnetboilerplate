using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Abp.Web.Dependency;
using Abp.Web.Dependency.Installers;
using Abp.Web.Dependency.Interceptors;
using Castle.Core;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Abp.Web.Startup
{
    public abstract class AbpBootstrapper : IDisposable
    {
        protected WindsorContainer WindsorContainer { get; private set; }

        protected AbpBootstrapper()
        {
            WindsorContainer = new WindsorContainer();
            WindsorContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        public void Initialize()
        {
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(WindsorContainer.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(WindsorContainer));

            RegisterInstallers();
            AutoMappingManager.Map();
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
            WindsorContainer.Install(new AbpInstaller());
        }

        public void Dispose()
        {
            if (WindsorContainer != null)
            {
                WindsorContainer.Dispose();
            }
        }
    }
}
