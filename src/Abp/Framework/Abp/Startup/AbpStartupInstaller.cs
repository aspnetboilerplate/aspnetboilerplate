using Abp.Localization.Sources;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Startup
{
    internal class AbpStartupInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterModuleSystem(container, store);
            RegisterLocalizationSystem(container, store);
            RegisterStartupManager(container, store);
        }

        protected virtual void RegisterModuleSystem(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpModuleCollection>().LifestyleSingleton(),
                Component.For<AbpModuleManager>().LifestyleSingleton(),
                Component.For<AbpModuleLoader>().LifestyleTransient()
                );
        }

        protected virtual void RegisterLocalizationSystem(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILocalizationSourceManager>().ImplementedBy<LocalizationSourceManager>().LifestyleSingleton()
                );
        }

        protected virtual void RegisterStartupManager(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpStartupManager>().LifestyleSingleton()
                );
        }
    }
}
