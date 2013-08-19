using System.Configuration;
using System.Reflection;
using Abp.Data.Dependency.Installers;
using Abp.Web.Startup;
using Castle.Windsor.Installer;
using ExamCenter.Entities.NHibernate.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace ExamCenter.Web.App_Start
{
    public class ExamCenterBootstrapper : AbpBootstrapper
    {
        protected override void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            base.ComponentRegistered(key, handler);

            NHibernateUnitOfWorkRegistrer.ComponentRegistered(key, handler);
        }

        protected override void RegisterInstallers()
        {
            base.RegisterInstallers();

            WindsorContainer.Install(new NHibernateInstaller(CreateNhSessionFactory)); // TODO: Move register event handler out and install below!
            WindsorContainer.Install(FromAssembly.This());
        }

        private static ISessionFactory CreateNhSessionFactory()
        {
            var connStr = "Server=localhost; Database=ExamCenter; Trusted_Connection=True;";
            //ConfigurationManager.ConnectionStrings["ExamCenterDb"].ConnectionString;
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(QuestionMap))))
                .BuildSessionFactory();
        }
    }
}