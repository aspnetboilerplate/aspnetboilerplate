using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Application.Authorization
{
    public class AuthorizationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AuthorizationInterceptor>().LifestyleTransient(),
                Classes.FromThisAssembly().BasedOn<ISingletonDependency>().WithService.DefaultInterfaces().WithService.Self().LifestyleSingleton()
                );
        }
    }
}
