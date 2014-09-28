using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Startup.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Startup
{
    internal class AbpStartupInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterStartupSystem(container, store);
            RegisterLocalizationSystem(container, store);
        }

        protected virtual void RegisterStartupSystem(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAbpStartupConfiguration>().ImplementedBy<AbpStartupConfiguration>().LifestyleSingleton(),
                Component.For<IAbpModuleManager>().ImplementedBy<AbpModuleManager>().LifestyleSingleton()
                );
        }

        protected virtual void RegisterLocalizationSystem(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILocalizationSourceManager>().ImplementedBy<LocalizationSourceManager>().LifestyleSingleton()
                );
        }
    }
}
