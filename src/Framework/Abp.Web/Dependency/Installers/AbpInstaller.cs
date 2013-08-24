using System.Web.Http;
using Abp.Web.Dependency.Interceptors;
using Castle.Core;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Abp.Web.Controllers;

namespace Abp.Web.Dependency.Installers
{
    public class AbpInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Log4Net
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());

            //Interceptors
            container.Register(
                Component.For<AbpApiControllerInterceptor>().LifeStyle.Transient,
                Classes.FromThisAssembly().BasedOn<AbpApiController>().LifestyleTransient()
                );
        }
    }
}
