using System;
using Abp.Data.Dependency.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;

namespace Abp.Data.Dependency.Installers
{
    /// <summary>
    /// This class installs base NHibernate components.
    /// TODO: Make sessionFactoryCreator mechanism better!
    /// </summary>
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