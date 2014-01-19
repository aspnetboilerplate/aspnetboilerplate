using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;
using Abp.WebApi.Dependency.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.WebApi.Dependency.Installers
{
    public class AbpWebApiInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpApiControllerInterceptor>().LifeStyle.Transient,
                Classes.FromThisAssembly().BasedOn<AbpApiController>().LifestyleTransient(),
                Component.For<ILocalizationScriptManager>().ImplementedBy<LocalizationScriptManager>().LifestyleSingleton()
                );
        }
    }
}
