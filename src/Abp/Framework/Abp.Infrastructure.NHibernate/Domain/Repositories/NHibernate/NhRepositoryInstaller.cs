using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.Domain.Repositories.NHibernate
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

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(() => _sessionFactory).LifeStyle.Singleton,

                //Generic repositories (So, user can directly inject a IRepository<TEntity> or IRepository<TEntity,TPrimaryKey>) without defining a class for it.
                Component.For(typeof (IRepository<>), typeof (NhRepositoryBase<>)).ImplementedBy(typeof (NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof (IRepository<,>), typeof (NhRepositoryBase<,>)).ImplementedBy(typeof (NhRepositoryBase<,>)).LifestyleTransient()

                );
        }
    }
}
