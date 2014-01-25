using System.Configuration;
using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Startup.Infrastructure.NHibernate;
using FluentNHibernate.Cfg.Db;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Infrastructure.Data.NHibernate", Dependencies = new[] { "Abp.Modules.Core.Infrastructure.NHibernate" })]
    public class TaskeverDataModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            var connStr = ConfigurationManager.ConnectionStrings["Taskever"].ConnectionString;
            initializationContext.GetModule<AbpNHibernateModule>().Configuration
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}