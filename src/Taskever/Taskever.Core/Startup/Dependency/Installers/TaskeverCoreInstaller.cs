using System.Reflection;
using Abp.Domain.Policies;
using Abp.Domain.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Friendships;
using Taskever.Localization.Resources;

namespace Taskever.Startup.Dependency.Installers
{
    public class TaskeverCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                
                Component.For<TaskeverLocalizationSource>().LifestyleSingleton(),

                Classes.FromAssembly(Assembly.GetAssembly(typeof (FriendshipPolicy)))
                    .BasedOn<IPolicy>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient(),

                Classes.FromThisAssembly()
                    .BasedOn<IDomainService>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient()

                );
        }
    }
}
