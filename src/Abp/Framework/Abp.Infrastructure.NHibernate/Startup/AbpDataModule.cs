using System;
using System.Collections.Generic;
using System.Configuration;
using Abp.Domain.Repositories.NHibernate;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.Startup;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Abp.Domain.Startup
{
    [AbpModule("Abp.Infrastructure.NHibernate")]
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
            NHibernateUnitOfWorkRegistrer.Initialize(initializationContext);
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            // TODO: Move register event handler out and install below!
            initializationContext.IocContainer.Install(new NhUnitOfWorkInstaller());
            initializationContext.IocContainer.Install(new NhRepositoryInstaller(CreateNhSessionFactory));
        }

        private ISessionFactory CreateNhSessionFactory()
        {
            //TODO: Move this to the application!
            var connStr = ConfigurationManager.ConnectionStrings["Taskever"].ConnectionString; //"Server=localhost; Database=Taskever; Trusted_Connection=True;";
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m =>
                              {
                                  foreach (var mapping in _mappings)
                                  {
                                      mapping(m);
                                  }
                              })
                //.Cache(c => c.ProviderClass<MemCache>().UseSecondLevelCache()) //TODO: Cache!
                .BuildSessionFactory();
        }
    }
}
