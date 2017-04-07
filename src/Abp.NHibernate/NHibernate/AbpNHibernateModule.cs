using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.NHibernate.Configuration;
using Abp.NHibernate.Filters;
using Abp.NHibernate.Interceptors;
using Abp.NHibernate.Repositories;
using Abp.NHibernate.Uow;
using NHibernate;

namespace Abp.NHibernate
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpNHibernateModule : AbpModule
    {
        /// <summary>
        /// NHibernate session factory object.
        /// </summary>
        private ISessionFactory _sessionFactory;

        public override void PreInitialize()
        {
            IocManager.Register<IAbpNHibernateModuleConfiguration, AbpNHibernateModuleConfiguration>();
            Configuration.ReplaceService<IUnitOfWorkFilterExecuter, NhUnitOfWorkFilterExecuter>(DependencyLifeStyle.Transient);
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.Register<AbpNHibernateInterceptor>(DependencyLifeStyle.Transient);

            _sessionFactory = Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Mappings(m => m.FluentMappings.Add(typeof(MayHaveTenantFilter)))
                .Mappings(m => m.FluentMappings.Add(typeof(MustHaveTenantFilter)))
                .ExposeConfiguration(config => config.SetInterceptor(IocManager.Resolve<AbpNHibernateInterceptor>()))
                .BuildSessionFactory();

            IocManager.IocContainer.Install(new NhRepositoryInstaller(_sessionFactory));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        /// <inheritdoc/>
        public override void Shutdown()
        {
            _sessionFactory.Dispose();
        }
    }
}
