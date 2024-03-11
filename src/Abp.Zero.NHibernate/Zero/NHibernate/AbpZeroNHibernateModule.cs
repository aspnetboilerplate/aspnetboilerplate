using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.EntityHistory;
using Abp.EntityHistory.EventListeners;
using Abp.Modules;
using Abp.NHibernate;
using NHibernate.Event;

namespace Abp.Zero.NHibernate
{
    /// <summary>
    /// Startup class for ABP Zero NHibernate module.
    /// </summary>
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpNHibernateModule))]
    public class AbpZeroNHibernateModule : AbpModule
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
                    m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())
                ).ExposeConfiguration(c =>
                {
                    c.AppendListeners(ListenerType.PreInsert, new[] { IocManager.Resolve<IPreInsertEventListener>() });
                    c.AppendListeners(ListenerType.PreUpdate, new[] { IocManager.Resolve<IPreUpdateEventListener>() });
                    c.AppendListeners(ListenerType.PreDelete, new[] { IocManager.Resolve<IPreDeleteEventListener>() });
                    c.AppendListeners(ListenerType.Flush, new[] { IocManager.Resolve<IFlushEventListener>() });
                });;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
