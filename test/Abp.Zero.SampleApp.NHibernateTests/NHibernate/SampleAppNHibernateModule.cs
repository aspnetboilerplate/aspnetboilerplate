using System;
using System.Data;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Zero.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

namespace Abp.Zero.SampleApp.NHibernate
{
    [DependsOn(typeof(AbpZeroNHibernateModule), typeof(SampleAppModule))]
    public class SampleAppNHibernateModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Database(SQLiteConfiguration.Standard.InMemory())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false, IocManager.Resolve<IDbConnection>(), Console.Out));
        }
    }
}
