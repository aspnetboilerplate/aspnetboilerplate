using Abp.Modules.Loading;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Dependency.Installers
{
    /// <summary>
    /// Registers dependencies for core (this) assembly.
    /// </summary>
    internal class AbpCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
            
            container.Register(
                Component.For<AbpModuleLoader>().LifestyleSingleton()
                );
        }
    }
}
