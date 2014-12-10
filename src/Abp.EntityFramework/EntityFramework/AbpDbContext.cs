using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using Abp.Configuration.Startup;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.EntityFramework.SoftDeleting;
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
        /// Constructor.
        /// Uses <see cref="IAbpStartupConfiguration.DefaultNameOrConnectionString"/> as connection string.
        /// </summary>
        protected AbpDbContext()
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbCompiledModel model)
            : base(model)
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            AbpSession = NullAbpSession.Instance;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Types<ISoftDelete>().Configure(c => c.HasTableAnnotation(AbpEfConsts.SoftDeleteCustomAnnotationName, true));

            // Using SoftDelete Attribute
            modelBuilder.Conventions.Add(
                new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
                    AbpEfConsts.SoftDeleteCustomAnnotationName,
                    (type, attributes) => attributes.Single().ColumnName)
                );
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry);
                        break;
                    case EntityState.Modified:
                        PreventSettingCreationAuditProperties(entry);
                        SetModificationAuditProperties(entry);
                        break;
                    case EntityState.Deleted:
                        PreventSettingCreationAuditProperties(entry);
                        HandleSoftDelete(entry);
                        break;
                }
            }

            return base.SaveChanges();
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
