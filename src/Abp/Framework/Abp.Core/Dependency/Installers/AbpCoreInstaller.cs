using Abp.Localization;
using Abp.Modules;
using Abp.Startup;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Dependency.Installers
{
    internal class AbpCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpModuleCollection>().LifestyleSingleton(),
                Component.For<AbpModuleManager>().LifestyleSingleton(),
                
                Component.For<AbpApplicationManager>().LifestyleSingleton(),

                Component.For<ILocalizationSourceManager>().ImplementedBy<LocalizationSourceManager>().LifestyleSingleton()
                );
        }
    }
}
