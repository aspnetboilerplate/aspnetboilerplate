using Abp.Localization.Sources;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Web.Localization
{
    /// <summary>
    /// Used to register Localization specific classes to IOC.
    /// </summary>
    public class LocalizationInstaller : IWindsorInstaller 
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpWebLocalizationSource>().LifestyleSingleton()
                );
        }
    }
}
