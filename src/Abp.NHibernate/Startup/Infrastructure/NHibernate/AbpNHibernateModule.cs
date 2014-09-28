using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Repositories.NHibernate;
using Abp.Domain.Repositories.NHibernate.Interceptors;
using Abp.Modules;
using FluentNHibernate.Cfg;
using NHibernate;

namespace Abp.Startup.Infrastructure.NHibernate
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    public class AbpNHibernateModule : AbpModule
    {
        /// <summary>
        /// NHibernate session factory object.
        /// </summary>
        private ISessionFactory _sessionFactory;
        
        public override void Initialize()
        {
            base.Initialize();

            _sessionFactory = Configuration.Modules.AbpNHibernate().FluentConfiguration
                .ExposeConfiguration(config => config.SetInterceptor(new AbpNHibernateInterceptor(IocManager)))
                .BuildSessionFactory();

            IocManager.IocContainer.Install(new NhRepositoryInstaller(_sessionFactory));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _sessionFactory.Dispose();
        }
    }
}
