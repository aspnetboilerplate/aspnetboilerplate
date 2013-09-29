using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taskever.Dependency.Installers
{
    public class TaskeverDataInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All repoistories //TODO: Make a custom repository example
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(NhTaskRepository))).InSameNamespaceAs<NhTaskRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                );
        }
    }
}