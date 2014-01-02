using System.Configuration;
using System.Reflection;
using Abp.Domain.Startup;
using Abp.Domain.Startup.NHibernate;
using Abp.Modules;
using Abp.Startup;
using FluentNHibernate.Cfg.Db;
using Taskever.Data.Repositories;
using Taskever.Dependency.Installers;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Infrastructure.Data.NHibernate", Dependencies = new[] { "Abp.Modules.Core.Infrastructure.NHibernate" })]
    public class TaskeverDataModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);

            initializationContext.IocContainer.Install(new RepositoryInstaller());

            var connStr = ConfigurationManager.ConnectionStrings["Taskever"].ConnectionString;

            initializationContext.GetModule<AbpNHibernateModule>().Configuration
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverDataInstaller());
        }
    }
}