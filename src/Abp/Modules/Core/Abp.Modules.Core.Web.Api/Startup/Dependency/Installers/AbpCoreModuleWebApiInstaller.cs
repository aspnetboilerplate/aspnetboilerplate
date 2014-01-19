using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Startup.Dependency.Installers
{
    public class AbpCoreModuleWebApiInstaller : IWindsorInstaller 
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All Web Api Controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient()

                );
        }
    }
}
