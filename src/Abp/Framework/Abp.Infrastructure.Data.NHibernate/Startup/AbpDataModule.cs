using System;
using System.Collections.Generic;
using Abp.Data.Dependency.Installers;
using Abp.Modules;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Abp.Data.Startup
{
    [AbpModule("Abp.Infrastructure.Data.NHibernate")]
    public class AbpDataModule : AbpModule
    {
        private readonly List<Action<MappingConfiguration>> _mappings;

        public AbpDataModule()
        {
            _mappings = new List<Action<MappingConfiguration>>();
        }

        public void AddMapping(Action<MappingConfiguration> mapping)
        {
            _mappings.Add(mapping);
        }

        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new NHibernateInstaller(CreateNhSessionFactory, initializationContext.ApplicationDirectory)); // TODO: Move register event handler out and install below!
        }

        protected void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            NHibernateUnitOfWorkRegistrer.ComponentRegistered(key, handler);
        }

        private ISessionFactory CreateNhSessionFactory()
        {
            //TODO: Move this to the application!
            var connStr = "Server=localhost; Database=Taskever; Trusted_Connection=True;";
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m =>
                              {
                                  foreach (var mapping in _mappings)
                                  {
                                      mapping(m);
                                  }
                              })
                .BuildSessionFactory();
        }
    }
}
