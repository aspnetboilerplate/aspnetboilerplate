using System;
using System.Data.Common;
using System.Reflection;

using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.NHibernate;
using Abp.TestBase;

using DapperExtensions.Sql;

using FluentNHibernate.Cfg.Db;

using NHibernate.Tool.hbm2ddl;

namespace Abp.Dapper.NHibernate.Tests
{
    [DependsOn(
        typeof(AbpNHibernateModule),
        typeof(AbpDapperModule),
        typeof(AbpTestBaseModule)
    )]
    public class AbpDapperNhBasedTestModule : AbpModule
    {
        private readonly object _lockObject = new object();

        public override void PreInitialize()
        {
            lock (_lockObject)
            {
                DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();
            }

            Configuration.Modules.AbpNHibernate().FluentConfiguration
                         .Database(SQLiteConfiguration.Standard.InMemory())
                         .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                         .ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false, IocManager.Resolve<DbConnection>(), Console.Out));
        }
    }
}
