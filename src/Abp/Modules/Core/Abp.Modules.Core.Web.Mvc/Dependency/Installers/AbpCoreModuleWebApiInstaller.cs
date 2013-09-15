using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Dependency.Installers
{
    public class AbpCoreModuleWebMvcInstaller : IWindsorInstaller 
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All MVC Controllers
                Classes.FromThisAssembly().BasedOn<Controller>().LifestyleTransient()

                );
        }
    }
}
