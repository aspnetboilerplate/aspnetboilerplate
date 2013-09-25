using System.Reflection;
using Abp.Localization;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Domain.Services.Impl;
using Taskever.Services.Impl;

namespace Taskever.Dependency.Installers
{
    public class TaskeverDataInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All repoistories //TODO: Make a custom repository example
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(NhTaskRepository))).InSameNamespaceAs<NhTaskRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services TODO: Move these to core!
                Classes.FromAssembly(Assembly.GetAssembly(typeof(TaskService))).InSameNamespaceAs<TaskService>().WithService.DefaultInterfaces().LifestyleTransient(),
                Classes.FromAssembly(Assembly.GetAssembly(typeof(TaskPrivilegeService))).InSameNamespaceAs<TaskPrivilegeService>().WithService.DefaultInterfaces().LifestyleTransient(),

                //Localization manager
                Component.For<ILocalizationManager>().ImplementedBy<NullLocalizationManager>().LifestyleSingleton()

                );
        }
    }
}