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
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Abp.Web.Startup
{
    public class AbpBootstrapper : IDisposable
    {
        protected WindsorContainer WindsorContainer { get; private set; }

        public void Initialize()
        {
            WindsorContainer = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(WindsorContainer.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(WindsorContainer));

            RegisterInstallers();
        }

        protected virtual void RegisterInstallers()
        {
            WindsorContainer.Install(new LoggerInstaller());
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
