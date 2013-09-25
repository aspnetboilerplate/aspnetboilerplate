using System.Reflection;
using Abp.Modules.Core.Application.Services.Impl;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Startup.Dependency
{
    public class AbpCoreModuleDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All services //TODO: Move to Abp.Modules.Core?
                Classes.FromAssembly(Assembly.GetAssembly(typeof(UserService))).InSameNamespaceAs<UserService>().WithService.DefaultInterfaces().LifestyleTransient()

                );
        }
    }
}
