using Abp.Web.Dependency.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Web.Dependency.Installers
{
    public class AbpWebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
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
