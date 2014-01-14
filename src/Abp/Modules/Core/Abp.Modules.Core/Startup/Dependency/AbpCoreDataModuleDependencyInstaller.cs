using System.Reflection;
using Abp.Modules.Core.Domain.Services.Impl;
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

                //All services
                Classes.FromAssembly(Assembly.GetAssembly(typeof(EmailService))).InSameNamespaceAs<EmailService>().WithService.DefaultInterfaces().LifestyleTransient()

                );
        }
    }
}
