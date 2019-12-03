using Abp.Domain.Repositories;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.NHibernate.Repositories
{
    internal class NhRepositoryInstaller : IWindsorInstaller
    {
        private readonly ISessionFactory _sessionFactory;

        public NhRepositoryInstaller(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ISessionFactory>().Instance(_sessionFactory).LifeStyle.Singleton,
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(NhRepositoryBase<,>)).LifestyleTransient()
            );
        }
    }
}
