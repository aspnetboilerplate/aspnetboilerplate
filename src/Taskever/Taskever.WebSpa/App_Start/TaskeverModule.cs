using System.Reflection;
using Abp.Data.Dependency.Installers;
using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Abp.Web.Controllers.Dynamic;
using Abp.Web.Modules;
using Castle.Windsor.Installer;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Taskever.Entities.NHibernate.Mappings;
using Taskever.Services;
using Taskever.Web.Dependency;

namespace Taskever.Web.App_Start
{
    [AbpModule("Taskever", Dependencies = new[] { "Abp.Modules.Core" })]
    public class TaskeverModule : AbpModule
    {
        public override void PreInitialize(AbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        public override void Initialize(AbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            initializationContext.IocContainer.Install(new NHibernateInstaller(CreateNhSessionFactory)); // TODO: Move register event handler out and install below!
            initializationContext.IocContainer.Install(FromAssembly.This());

            AutoMappingManager.Map();

            DynamicControllerGenerator.GenerateFor<ITaskService>(); //TODO: where to write?

        }

        protected void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
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