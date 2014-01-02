using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Domain.Uow.NHibernate
{
    internal class NhUnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<NhUnitOfWorkInterceptor>().LifeStyle.Transient
                );
        }
    }
}
