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
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            //Register all components
            container.Register(

                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(_sessionFactoryCreator).LifeStyle.Singleton,

                //Unitofwork interceptor
                Component.For<NhUnitOfWorkInterceptor>().LifeStyle.Transient

                ////All repoistories
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(NhPersonRepository))).InSameNamespaceAs<NhPersonRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                ////All services
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(PersonService))).InSameNamespaceAs<PersonService>().WithService.DefaultInterfaces().LifestyleTransient()

                );
        }

        void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            //Intercept all methods of all repositories.
            if (UnitOfWorkHelper.IsRepositoryClass(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(NhUnitOfWorkInterceptor)));
            }

            //Intercept all methods of classes those have at least one method that has UnitOfWork attribute.
            foreach (var method in handler.ComponentModel.Implementation.GetMethods())
            {
                if (UnitOfWorkHelper.HasUnitOfWorkAttribute(method))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(NhUnitOfWorkInterceptor)));
                    return;
                }
            }
        }
    }
}