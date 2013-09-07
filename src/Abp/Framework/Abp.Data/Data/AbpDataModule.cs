using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Data.Dependency.Installers;
using Abp.Modules;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Abp.Data
{
    [AbpModule("Abp.Data")]
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
            initializationContext.IocContainer.Install(new NHibernateInstaller(CreateNhSessionFactory)); // TODO: Move register event handler out and install below!
        }

        protected void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            NHibernateUnitOfWorkRegistrer.ComponentRegistered(key, handler);
        }

        private ISessionFactory CreateNhSessionFactory()
        {
            var connStr = "Server=localhost; Database=Taskever; Trusted_Connection=True;";
            //ConfigurationManager.ConnectionStrings["TaskeverDb"].ConnectionString;
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
