using System;
using System.Data.Common;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.TestBase;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate.Tool.hbm2ddl;

namespace Abp.NHibernate.Tests
{
    [DependsOn(typeof(AbpNHibernateModule), typeof(AbpTestBaseModule))]
    public class NHibernateTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Database(SQLiteConfiguration.Standard.InMemory())
                .Mappings(m =>
                    m.FluentMappings
                        .Conventions.Add(
                           DynamicInsert.AlwaysTrue(),
                           DynamicUpdate.AlwaysTrue()
                        )
                        .AddFromAssembly(Assembly.GetExecutingAssembly())
                ).ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false, IocManager.Resolve<DbConnection>(), Console.Out));
        }
    }
}