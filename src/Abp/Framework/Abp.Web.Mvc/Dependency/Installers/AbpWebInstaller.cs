using Abp.Web.Mvc.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Web.Mvc.Dependency.Installers
{
    public class AbpWebMvcInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<AbpController>().LifestyleTransient()
                );
        }
    }
}
