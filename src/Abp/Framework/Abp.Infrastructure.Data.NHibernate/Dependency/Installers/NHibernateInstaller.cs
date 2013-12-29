using System;
using System.IO;
using System.Reflection;
using Abp.Data.Dependency.Interceptors;
using Abp.Data.Repositories.NHibernate;
using Abp.Domain.Repositories;
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
        private readonly string _applicationDirectory;

        public NHibernateInstaller(Func<ISessionFactory> sessionFactoryCreator, string applicationDirectory)
        {
            _sessionFactoryCreator = sessionFactoryCreator;
            _applicationDirectory = applicationDirectory;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Register all components
            container.Register(

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(_sessionFactoryCreator).LifeStyle.Singleton,

                //Unitofwork interceptor
                Component.For<NhUnitOfWorkInterceptor>().LifeStyle.Transient,

                //Generic repositories
                Component.For(typeof(IRepository<>), typeof(NhRepositoryBase<>)).ImplementedBy(typeof(NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof(IRepository<,>), typeof(NhRepositoryBase<,>)).ImplementedBy(typeof(NhRepositoryBase<,>)).LifestyleTransient(),

                //TODO: Is this true to register all repositories. Think it?
                Classes.FromAssemblyInDirectory(new AssemblyFilter(_applicationDirectory)).BasedOn(typeof(IRepository<,>)).WithServiceDefaultInterfaces().LifestyleTransient().WithServiceSelf().LifestyleTransient()

                );
        }
    }
}