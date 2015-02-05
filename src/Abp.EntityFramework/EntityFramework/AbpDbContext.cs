using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Abp.Configuration.Startup;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Session;

namespace Abp.EntityFramework
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class AbpDbContext : DbContext
    {
        /// <summary>
        /// Used to get current session values.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Used to trigger events.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to the unit of work manager.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        /// <summary>
        /// Constructor.
        /// Uses <see cref="IAbpStartupConfiguration.DefaultNameOrConnectionString"/> as connection string.
        /// </summary>
        protected AbpDbContext()
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbCompiledModel model)
            : base(model)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Types<ISoftDelete>().Configure(c => c.HasTableAnnotation(AbpEfConsts.SoftDeleteCustomAnnotationName, true));
        }
        
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry);
                        //TriggerEntityCreatedEvent(entry.Entity); //not implemented yet
                        break;
                    case EntityState.Modified:
                        PreventSettingCreationAuditProperties(entry);
                        SetModificationAuditProperties(entry);
                        //TriggerEntityUpdatedEvent(entry.Entity); //not implemented yet
                        break;
                    case EntityState.Deleted:
                        PreventSettingCreationAuditProperties(entry);
                        HandleSoftDelete(entry);
                        break;
                }
            }

            return base.SaveChanges();
        }

        private void TriggerEntityUpdatedEvent(object entity)
        {
            var entityType = entity.GetType();
            var eventType = typeof(EntityUpdatedEventData<>).MakeGenericType(entityType);

            if (UnitOfWorkManager == null || UnitOfWorkManager.Current == null)
            {
                EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
                return;
            }

            UnitOfWorkManager.Current.Completed += (sender, args) => EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }

        private void TriggerEntityCreatedEvent(object entity)
        {
            var entityType = entity.GetType();
            var eventType = typeof (EntityCreatedEventData<>).MakeGenericType(entityType);
            
            if (UnitOfWorkManager == null || UnitOfWorkManager.Current == null)
            {
                EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
                return;
            }

            UnitOfWorkManager.Current.Completed += (sender, args) => EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }

        private void SetCreationAuditProperties(DbEntityEntry entry)
        {
            if (entry.Entity is IHasCreationTime)
            {
                entry.Cast<IHasCreationTime>().Entity.CreationTime = DateTime.Now; //TODO: UtcNow?
            }

            if (entry.Entity is ICreationAudited)
            {
                entry.Cast<ICreationAudited>().Entity.CreatorUserId = AbpSession.UserId;
            }
        }

        private void PreventSettingCreationAuditProperties(DbEntityEntry entry)
        {
            //TODO@Halil: Implement this when tested well (Issue #49)
            //if (entry.Entity is IHasCreationTime && entry.Cast<IHasCreationTime>().Property(e => e.CreationTime).IsModified)
            //{
            //    throw new DbEntityValidationException(string.Format("Can not change CreationTime on a modified entity {0}", entry.Entity.GetType().FullName));
            //}

            //if (entry.Entity is ICreationAudited && entry.Cast<ICreationAudited>().Property(e => e.CreatorUserId).IsModified)
            //{
            //    throw new DbEntityValidationException(string.Format("Can not change CreatorUserId on a modified entity {0}", entry.Entity.GetType().FullName));
            //}
        }

        private void SetModificationAuditProperties(DbEntityEntry entry)
        {
            if (entry.Entity is IModificationAudited)
            {
                var auditedEntry = entry.Cast<IModificationAudited>();

                auditedEntry.Entity.LastModificationTime = DateTime.Now; //TODO: UtcNow?
                auditedEntry.Entity.LastModifierUserId = AbpSession.UserId;
            }
        }

        private void HandleSoftDelete(DbEntityEntry entry)
        {
            if (entry.Entity is ISoftDelete)
            {
                var softDeleteEntry = entry.Cast<ISoftDelete>();

                softDeleteEntry.State = EntityState.Unchanged;
                softDeleteEntry.Entity.IsDeleted = true;
                
                if (entry.Entity is IDeletionAudited)
                {
                    var deletionAuditedEntry = entry.Cast<IDeletionAudited>();
                    deletionAuditedEntry.Entity.DeletionTime = DateTime.Now; //TODO: UtcNow?
                    deletionAuditedEntry.Entity.DeleterUserId = AbpSession.UserId;
                }
            }
        }
    }
}
