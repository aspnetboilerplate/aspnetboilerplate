using Abp.Localization;
using Abp.Modules.Loading;
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
            container.Register(
                Component.For<AbpModuleLoader>().LifestyleSingleton(),
                Component.For<ILocalizationManager>().ImplementedBy<LocalizationManager>().LifestyleSingleton()
                );
        }
    }
}
