using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Abp.NHibernate.Tests
{
    [DependsOn(typeof(AbpNHibernateModule))]
    public class NHibernateTestModule : AbpModule
    {

        public override void PreInitialize()
        {
            System.Data.SQLite.SQLiteConnection c = null;

            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Database(SQLiteConfiguration.Standard.InMemory())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .ExposeConfiguration(cfg =>
                                     {
                                         //This is not working since it works for a single session!
                                         //Instead, find a way of working per ISessionFactory.
                                         cfg.SetProperty("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
                                         cfg.SetProperty(Environment.ReleaseConnections, "on_close");
                                         new SchemaExport(cfg).Create(true, true);
                                     });
        }
    }
}