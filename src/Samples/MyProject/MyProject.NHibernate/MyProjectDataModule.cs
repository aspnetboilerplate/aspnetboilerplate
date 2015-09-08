using System.Configuration;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.NHibernate;
using FluentNHibernate.Cfg.Db;

namespace MyProject
{
    [DependsOn(typeof(AbpNHibernateModule), typeof(MyProjectCoreModule))]
    public class MyProjectDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = ConfigurationManager.ConnectionStrings["Default_MySQL"].ConnectionString;
            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Database(MySQLConfiguration.Standard.ConnectionString(Configuration.DefaultNameOrConnectionString))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}