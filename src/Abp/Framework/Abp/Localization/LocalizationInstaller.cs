using Abp.Localization.Sources;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Localization
{
    /// <summary>
    /// Used to register Localization specific classes to IOC.
    /// </summary>
    public class LocalizationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILocalizationSourceManager>().ImplementedBy<LocalizationSourceManager>().LifestyleSingleton()
                );
        }
    }
}
