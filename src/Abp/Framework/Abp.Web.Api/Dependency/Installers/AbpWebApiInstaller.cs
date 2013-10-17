using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Filters;
using Abp.WebApi.Dependency.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.WebApi.Dependency.Installers
{
    public class AbpWebApiInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Interceptors
            container.Register(

                Component.For<AbpApiControllerInterceptor>().LifeStyle.Transient,

                Component.For<AbpExceptionFilterAttribute>().LifeStyle.Transient,

                //All api controllers //TODO: No need it, there is no such a class.
                Classes.FromThisAssembly().BasedOn<AbpApiController>().LifestyleTransient()

                );
        }
    }
}
