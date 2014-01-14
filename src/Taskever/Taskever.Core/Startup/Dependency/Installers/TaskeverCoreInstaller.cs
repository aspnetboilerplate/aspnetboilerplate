using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Localization.Resources;

namespace Taskever.Startup.Dependency.Installers
{
    public class TaskeverCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<TaskeverLocalizationSource>().LifestyleSingleton()
                );
        }
    }
}
