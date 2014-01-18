using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules
{
    public class AbpModuleSystemInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpModuleCollection>().LifestyleSingleton(),
                Component.For<AbpModuleManager>().LifestyleSingleton(),
                Component.For<AbpModuleLoader>().LifestyleTransient()
                );
        }
    }
}
