using Abp.Application.Authorization.Permissions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Application.Authorization
{
    internal class AuthorizationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AuthorizationInterceptor>().LifestyleTransient(),
                Component.For<IPermissionManager>().ImplementedBy<PermissionManager>().LifestyleSingleton()
                );
        }
    }
}
