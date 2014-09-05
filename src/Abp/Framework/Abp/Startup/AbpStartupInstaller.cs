using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Startup
{
    /// <summary>
    /// Asp.Net BoilerPlate startup installer class.
    /// </summary>
    public class AbpStartupInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<AbpApplicationManager>().LifestyleSingleton());
        }
    }
}
