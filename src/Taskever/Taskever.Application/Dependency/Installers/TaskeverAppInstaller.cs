using System.Reflection;
using Abp.Localization;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Application.Services.Impl;
using Taskever.Domain.Policies.Impl;
using Taskever.Domain.Services.Impl;

namespace Taskever.Dependency.Installers
{
    public class TaskeverAppInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //TODO: Need to register WithServiceSelf()?
            container.Register(
                Classes.FromAssembly(Assembly.GetAssembly(typeof(TaskService))).InSameNamespaceAs<TaskService>().WithService.DefaultInterfaces().LifestyleTransient(),
                Classes.FromAssembly(Assembly.GetAssembly(typeof(TaskPrivilegeService))).InSameNamespaceAs<TaskPrivilegeService>().WithService.DefaultInterfaces().LifestyleTransient(),
                Classes.FromAssembly(Assembly.GetAssembly(typeof(FriendshipPolicy))).InSameNamespaceAs<FriendshipPolicy>().WithService.DefaultInterfaces().LifestyleTransient()
                );
        }
    }
}