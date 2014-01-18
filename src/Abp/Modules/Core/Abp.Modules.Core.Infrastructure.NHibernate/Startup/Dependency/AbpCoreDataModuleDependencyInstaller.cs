using Abp.Modules.Core.Data.Repositories.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Startup.Dependency
{
    public class AbpCoreDataModuleDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Component.For(typeof(AuditInterceptor)),
                Component.For(typeof(MultiTenancyInterceptor<,>))

                );
        }
    }
}
