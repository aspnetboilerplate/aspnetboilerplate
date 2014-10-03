using System.Reflection;
using Abp.Modules;
using Abp.NHibernate.Config;
using Abp.NHibernate.Interceptors;
using Abp.NHibernate.Repositories;
using NHibernate;

namespace Abp.NHibernate
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
            _sessionFactory = Configuration.Modules.AbpNHibernate().FluentConfiguration
                .ExposeConfiguration(config => config.SetInterceptor(new AbpNHibernateInterceptor(IocManager)))
                .BuildSessionFactory();

            IocManager.IocContainer.Install(new NhRepositoryInstaller(_sessionFactory));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void Shutdown()
        {
            _sessionFactory.Dispose();
        }
    }
}
