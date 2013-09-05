using System;
using System.Reflection;
using Abp.Data.Dependency.Installers;
using Abp.Entities.NHibernate.Mappings.Core;
using Abp.Web.Controllers.Dynamic;
using Abp.Web.Startup;
using Castle.Windsor.Installer;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Taskever.Entities.NHibernate.Mappings;
using Taskever.Services;

namespace Taskever.Web.Dependency
{
    public class TaskeverBootstrapper : AbpBootstrapper
    {
        protected override void RegisterInstallers()
        {
            base.RegisterInstallers();

            IocContainer.Install(new NHibernateInstaller(CreateNhSessionFactory)); // TODO: Move register event handler out and install below!
            IocContainer.Install(FromAssembly.This());

            Abp.Web.Startup.AutoMappingManager.Map();
            AutoMappingManager.Map();
            
            DynamicControllerMapper.Map<ITaskService>(); //TODO: where to write?
        }

        protected override void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            base.ComponentRegistered(key, handler);

            NHibernateUnitOfWorkRegistrer.ComponentRegistered(key, handler);
        }

        private static ISessionFactory CreateNhSessionFactory()
        {
            var connStr = "Server=localhost; Database=Taskever; Trusted_Connection=True;";
            //ConfigurationManager.ConnectionStrings["TaskeverDb"].ConnectionString;
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(
                    m => m.FluentMappings
                             .AddFromAssembly(Assembly.GetAssembly(typeof(TaskMap)))
                             .AddFromAssembly(Assembly.GetAssembly(typeof(UserMap))))
                .BuildSessionFactory();
        }
    }
}