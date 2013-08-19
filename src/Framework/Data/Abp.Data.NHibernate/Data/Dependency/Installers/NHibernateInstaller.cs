using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.Data.Dependency.Installers
{
    public class NHibernateInstaller : IWindsorInstaller
    {
        private readonly Func<ISessionFactory> _sessionFactoryCreator;

        public NHibernateInstaller(Func<ISessionFactory> sessionFactoryCreator)
        {
            _sessionFactoryCreator = sessionFactoryCreator;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Register all components
            container.Register(

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(_sessionFactoryCreator).LifeStyle.Singleton,

                //Unitofwork interceptor
                Component.For<NhUnitOfWorkInterceptor>().LifeStyle.Transient

                );
        }
    }
}