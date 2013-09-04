using System;
using System.Reflection;
using Abp.Data.Dependency.Installers;
using Abp.Entities.NHibernate.Mappings.Core;
using Abp.Services;
using Abp.Web.Startup;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Taskever.Entities.NHibernate.Mappings;
using Taskever.Web.Api;
using AutoMappingManager = Taskever.Web.Dependency.AutoMappingManager;

namespace Taskever.Web.Dependency
{
    public class TaskeverBootstrapper : AbpBootstrapper
    {
        public void Map<T>() where T : IService
        {
            var proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
            proxyGenerator.CreateClassProxy(typeof (AbpServiceApiController), new [] {typeof (T)});

            WindsorContainer.Register(
                //Component.For()
                );
        }

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
            AutoMappingManager.Map();
        }

        private static ISessionFactory CreateNhSessionFactory()
        {
            var connStr = "Server=localhost; Database=Taskever; Trusted_Connection=True;";
            //ConfigurationManager.ConnectionStrings["TaskeverDb"].ConnectionString;
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(
                    m => m.FluentMappings
                             .AddFromAssembly(Assembly.GetAssembly(typeof (TaskMap)))
                             .AddFromAssembly(Assembly.GetAssembly(typeof (UserMap))))
                .BuildSessionFactory();
        }
    }
}