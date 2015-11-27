using System.Configuration;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using Abp.Zero;
using Abp.Domain.Uow;

namespace MyAbpZeroProject
{
    [DependsOn(typeof(AbpNHibernateModule), typeof(AbpZeroCoreModule))]
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
               .Database(MySQLConfiguration.Standard.Dialect<MySQL5Dialect>().ConnectionString(connStr).ShowSql())
               //.Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
               
               ;

            Configuration.MultiTenancy.IsEnabled = true;//是否启用多租户

            Configuration.UnitOfWork.OverrideFilter(AbpDataFilters.MustHaveTenant, false);
            Configuration.UnitOfWork.OverrideFilter(AbpDataFilters.MayHaveTenant, false);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
