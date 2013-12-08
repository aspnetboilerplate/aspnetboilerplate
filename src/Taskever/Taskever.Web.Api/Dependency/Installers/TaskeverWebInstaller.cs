using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taskever.Web.Dependency.Installers
{
    public class TaskeverWebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //TODO: Write a custom Web API to be a sample?
            container.Register(

                //All Web Api Controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient() //TODO: No need in fact

                );
        }
    }
}