using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.EntityHistory.EventListeners;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.NHibernate;
using Castle.MicroKernel.Registration;
using NHibernate.Event;
using System.Reflection;

namespace Abp.Zero.NHibernate;

/// <summary>
/// Startup class for ABP Zero NHibernate module.
/// </summary>
[DependsOn(typeof(AbpZeroCoreModule), typeof(AbpNHibernateModule))]
public class AbpZeroCoreNHibernateModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.ReplaceService<IEntityHistoryStore, NhEntityHistoryStore>(DependencyLifeStyle.Transient);

        IocManager.Register<IEntityHistoryHelper, NhEntityHistoryHelper>(DependencyLifeStyle.Transient);
        IocManager.Register<IPreInsertEventListener, InsertEventListener>(DependencyLifeStyle.Transient);
        IocManager.Register<IPreUpdateEventListener, UpdateEventListener>(DependencyLifeStyle.Transient);
        IocManager.Register<IPreDeleteEventListener, DeleteEventListener>(DependencyLifeStyle.Transient);
        IocManager.Register<IFlushEventListener, FlushEventListener>(DependencyLifeStyle.Transient);

        Configuration.Modules.AbpNHibernate().FluentConfiguration
            .Mappings(
                m => m.FluentMappings.AddFromAssemblyOf<AbpZeroCoreNHibernateModule>()
            )
            .ExposeConfiguration(c =>
            {
                c.AppendListeners(ListenerType.PreInsert, new[] { IocManager.Resolve<IPreInsertEventListener>() });
                c.AppendListeners(ListenerType.PreUpdate, new[] { IocManager.Resolve<IPreUpdateEventListener>() });
                c.AppendListeners(ListenerType.PreDelete, new[] { IocManager.Resolve<IPreDeleteEventListener>() });
                c.AppendListeners(ListenerType.Flush, new[] { IocManager.Resolve<IFlushEventListener>() });
            });


        Configuration.ReplaceService(typeof(IConnectionStringResolver), () =>
        {
            IocManager.IocContainer.Register(
                Component.For<IConnectionStringResolver, IDbPerTenantConnectionStringResolver>()
                    .ImplementedBy<DbPerTenantConnectionStringResolver>()
                    .LifestyleTransient()
            );
        });
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}