using System.Reflection;
using Abp.Application.Services;
using Abp.Domain.Policies;
using Abp.Domain.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Application.Services.Impl;
using Taskever.Friendships;

namespace Taskever.Dependency.Installers
{
    public class TaskeverAppInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Classes.FromAssembly(Assembly.GetAssembly(typeof (FriendshipPolicy)))
                    .BasedOn<IPolicy>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetAssembly(typeof (FriendshipDomainService)))
                    .BasedOn<IDomainService>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetAssembly(typeof (TaskService)))
                    .BasedOn<IApplicationService>()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                    .WithService.Self()
                    .LifestyleTransient()

                );
        }
    }
}