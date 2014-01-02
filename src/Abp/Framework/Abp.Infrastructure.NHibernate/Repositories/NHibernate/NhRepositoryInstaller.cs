using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.Domain.Repositories.NHibernate
{
    internal class NhRepositoryInstaller : IWindsorInstaller
    {
        private readonly Func<ISessionFactory> _sessionFactoryCreator;

        public NhRepositoryInstaller(Func<ISessionFactory> sessionFactoryCreator)
        {
            _sessionFactoryCreator = sessionFactoryCreator;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(_sessionFactoryCreator).LifeStyle.Singleton,
                
                //Generic repositories
                Component.For(typeof (IRepository<>), typeof (NhRepositoryBase<>)).ImplementedBy(typeof (NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof (IRepository<,>), typeof (NhRepositoryBase<,>)).ImplementedBy(typeof (NhRepositoryBase<,>)).LifestyleTransient()

                );
        }
    }
}
