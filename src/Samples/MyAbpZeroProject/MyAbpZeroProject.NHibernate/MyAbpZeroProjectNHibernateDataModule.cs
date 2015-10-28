using System.Reflection;
using Abp.Modules;
using Abp.Zero.NHibernate;
using FluentNHibernate.Cfg.Db;
using Abp.Configuration.Startup;
using System.Configuration;
using Abp.NHibernate;
using NHibernate.Dialect;

namespace MyAbpZeroProject
{
    [DependsOn(typeof(AbpZeroNHibernateModule), typeof(MyAbpZeroProjectCoreModule), typeof(AbpNHibernateModule))]
    public class MyAbpZeroProjectNHibernateDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Configuration.DefaultNameOrConnectionString = ConfigurationManager.ConnectionStrings["Default_Mysql"].ConnectionString;
            //Configuration.Modules.AbpNHibernate().FluentConfiguration
            //    .Database(MySQLConfiguration.Standard.ConnectionString(Configuration.DefaultNameOrConnectionString))
            //    .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
            //   // .Mappings(m => m.FluentMappings.AddFromAssembly(System.Reflection.Assembly.Load("Abp.Zero.NHibernate")))
            //    ;
           
            //MySQL
            var connStr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            Configuration.Modules.AbpNHibernate().FluentConfiguration
               .Database(MySQLConfiguration.Standard
    .Dialect<MySQL5Dialect>()
    .ConnectionString(connStr))
               
               .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));

            Configuration.MultiTenancy.IsEnabled = true;//是否启用多租户
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
