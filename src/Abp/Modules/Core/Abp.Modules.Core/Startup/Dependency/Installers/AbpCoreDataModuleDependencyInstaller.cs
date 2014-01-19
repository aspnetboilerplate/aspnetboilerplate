using Abp.Domain.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Startup.Dependency.Installers
{
    public class AbpCoreModuleDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<IDomainService>().WithService.DefaultInterfaces().WithService.Self().LifestyleTransient()
                );
        }
    }
}
