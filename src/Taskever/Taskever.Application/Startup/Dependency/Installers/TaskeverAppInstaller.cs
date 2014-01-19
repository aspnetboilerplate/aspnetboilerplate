using Abp.Application.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taskever.Startup.Dependency.Installers
{
    public class TaskeverAppInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IApplicationService>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient()
                );
        }
    }
}