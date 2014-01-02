using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.Domain.Repositories.NHibernate
{
    internal class NhRepositoryInstaller : IWindsorInstaller
    {
        private readonly ISessionFactory _sessionFactoryCreator;

        public NhRepositoryInstaller(ISessionFactory sessionFactory)
        {
            _sessionFactoryCreator = sessionFactory;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(() => _sessionFactoryCreator).LifeStyle.Singleton,

                //Generic repositories (So, user can directly instantiate a IRepository<TEntity> or IRepository<TEntity,TPrimaryKey>)
                Component.For(typeof (IRepository<>), typeof (NhRepositoryBase<>)).ImplementedBy(typeof (NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof (IRepository<,>), typeof (NhRepositoryBase<,>)).ImplementedBy(typeof (NhRepositoryBase<,>)).LifestyleTransient()

                );
        }
    }
}
