using Abp.Web.Dependency.Interceptors;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

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

                //ApiController interceptor
                Component.For<AbpApiControllerInterceptor>().LifeStyle.Transient

                //All api controllers //TODO ???
                //Classes.FromThisAssembly().BasedOn<AbpApiController>().LifestyleTransient()

                );
        }
    }
}
