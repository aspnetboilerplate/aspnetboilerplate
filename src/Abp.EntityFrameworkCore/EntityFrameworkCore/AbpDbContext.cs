using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Utils;
using Abp.EntityFrameworkCore.ValueConverters;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Linq.Expressions;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Abp.EntityFrameworkCore;

/// <summary>
/// Base class for all DbContext classes in the application.
/// </summary>
public abstract class AbpDbContext : DbContext, ITransientDependency, IShouldInitializeDcontext
{
    /// <summary>
    /// Used to get current session values.
    /// </summary>
    public IAbpSession AbpSession { get; set; }

    /// <summary>
    /// Used to trigger entity change events.
    /// </summary>
    public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

    /// <summary>
    /// Reference to the logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Reference to the event bus.
    /// </summary>
    public IEventBus EventBus { get; set; }

    /// <summary>
    /// Reference to GUID generator.
    /// </summary>
    public IGuidGenerator GuidGenerator { get; set; }

    /// <summary>
    /// Reference to the current UOW provider.
    /// </summary>
    public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

    /// <summary>
    /// Reference to multi tenancy configuration.
    /// </summary>
    public IMultiTenancyConfig MultiTenancyConfig { get; set; }

    /// <summary>
    /// Reference to the ABP entity configuration.
    /// </summary>
    public IAbpEfCoreConfiguration AbpEfCoreConfiguration { get; set; }

    /// <summary>
    /// Can be used to suppress automatically setting TenantId on SaveChanges.
    /// Default: false.
    /// </summary>
    public virtual bool SuppressAutoSetTenantId { get; set; }

    public virtual int? CurrentTenantId => GetCurrentTenantIdOrNull();

    public virtual bool IsSoftDeleteFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(AbpDataFilters.SoftDelete) == true;

    public virtual bool IsMayHaveTenantFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(AbpDataFilters.MayHaveTenant) == true;

    public virtual bool IsMustHaveTenantFilterEnabled => CurrentTenantId != null && CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled(AbpDataFilters.MustHaveTenant) == true;

    public virtual bool IsSoftDeleteCascadeEnabled => AbpEfCoreConfiguration?.EnableSoftDeleteCascade == true;

    private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(AbpDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

    private static MethodInfo ConfigureGlobalValueConverterMethodInfo = typeof(AbpDbContext).GetMethod(nameof(ConfigureGlobalValueConverter), BindingFlags.Instance | BindingFlags.NonPublic);

    protected readonly DbContextOptions DbContextOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected AbpDbContext(DbContextOptions options)
        : base(options)
    {
        DbContextOptions = options;
        InitializeDbContext();
    }

    private void InitializeDbContext()
    {
        SetNullsForInjectedProperties();
    }

    private void SetNullsForInjectedProperties()
    {
        Logger = NullLogger.Instance;
        AbpSession = NullAbpSession.Instance;
        EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
        GuidGenerator = SequentialGuidGenerator.Instance;
        EventBus = NullEventBus.Instance;
        AbpEfCoreConfiguration = NullAbpEfCoreConfiguration.Instance;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });

            ConfigureGlobalValueConverterMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }
    }

    protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
        where TEntity : class
    {
        if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
        {
            var filterExpression = CreateFilterExpression<TEntity>(modelBuilder);
            if (filterExpression != null)
            {
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }
    }

    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }

        if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }

        if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }

        return false;
    }

    protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = null;

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> softDeleteFilter = e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDeleted;
            if (UseAbpQueryCompiler())
            {
                softDeleteFilter = e => SoftDeleteFilter(((ISoftDelete)e).IsDeleted, true);
                modelBuilder.ConfigureSoftDeleteDbFunction(typeof(AbpDbContext).GetMethod(nameof(SoftDeleteFilter), new[] { typeof(bool), typeof(bool) })!, this.GetService<AbpEfCoreCurrentDbContext>());
            }
            expression = expression == null ? softDeleteFilter : CombineExpressions(expression, softDeleteFilter);
        }

        if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant)e).TenantId == CurrentTenantId;
            if (UseAbpQueryCompiler())
            {
                mayHaveTenantFilter = e => MayHaveTenantFilter(((IMayHaveTenant)e).TenantId, CurrentTenantId, true);
                modelBuilder.ConfigureMayHaveTenantDbFunction(typeof(AbpDbContext).GetMethod(nameof(MayHaveTenantFilter), new[] { typeof(int?), typeof(int?), typeof(bool) })!, this.GetService<AbpEfCoreCurrentDbContext>());
            }
            expression = expression == null ? mayHaveTenantFilter : CombineExpressions(expression, mayHaveTenantFilter);
        }

        if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant)e).TenantId == CurrentTenantId;
            if (UseAbpQueryCompiler())
            {
                mustHaveTenantFilter = e => MustHaveTenantFilter(((IMustHaveTenant)e).TenantId, CurrentTenantId, true);
                modelBuilder.ConfigureMustHaveTenantDbFunction(typeof(AbpDbContext).GetMethod(nameof(MustHaveTenantFilter), new[] { typeof(int), typeof(int?), typeof(bool) })!, this.GetService<AbpEfCoreCurrentDbContext>());
            }
            expression = expression == null ? mustHaveTenantFilter : CombineExpressions(expression, mustHaveTenantFilter);
        }

        return expression;
    }

    protected virtual bool UseAbpQueryCompiler()
    {
        return DbContextOptions?.FindExtension<AbpDbContextOptionsExtension>() != null && AbpEfCoreConfiguration.UseAbpQueryCompiler;
    }

    /// <summary>
    /// Returns a value that is appended to EF Core's compiled query cache key.
    /// The current tenant id is always sent to the database as a SQL parameter, so it is not part of the key.
    /// The filter states are part of the key only when <see cref="UseAbpQueryCompiler"/> is enabled,
    /// because only then are they inlined into the generated SQL.
    /// </summary>
    public virtual string GetCompiledQueryCacheKey()
    {
        if (!UseAbpQueryCompiler())
        {
            return string.Empty;
        }

        return $"{IsSoftDeleteFilterEnabled}:{IsMayHaveTenantFilterEnabled}:{IsMustHaveTenantFilterEnabled}";
    }

    protected const string DbFunctionNotSupportedExceptionMessage = "Your EF Core database provider does not support 'User-defined function mapping'." +
                                                        "Please set 'UseAbpQueryCompiler' of 'IAbpEfCoreConfiguration' to false to disable it." +
                                                        "See https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping for more information.";

    public static bool SoftDeleteFilter(bool isDeleted, bool boolParam)
    {
        throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
    }

    public static bool MustHaveTenantFilter(int tenantId, int? currentTenantId, bool boolParam)
    {
        throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
    }

    public static bool MayHaveTenantFilter(int? tenantId, int? currentTenantId, bool boolParam)
    {
        throw new NotSupportedException(DbFunctionNotSupportedExceptionMessage);
    }

    protected void ConfigureGlobalValueConverter<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
        where TEntity : class
    {
        if (entityType.BaseType == null &&
            !typeof(TEntity).IsDefined(typeof(DisableDateTimeNormalizationAttribute), true) &&
            !typeof(TEntity).IsDefined(typeof(OwnedAttribute), true) &&
            !entityType.IsOwned())
        {
            var dateTimeValueConverter = new AbpDateTimeValueConverter();
            var dateTimePropertyInfos = DateTimePropertyInfoHelper.GetDatePropertyInfos(typeof(TEntity));
            dateTimePropertyInfos.DateTimePropertyInfos.ForEach(property =>
            {
                modelBuilder
                    .Entity<TEntity>()
                    .Property(property.Name)
                    .HasConversion(dateTimeValueConverter);
            });
        }
    }

    public override int SaveChanges()
    {
        try
        {
            var changeReport = ApplyAbpConcepts();
            var result = base.SaveChanges();
            EntityChangeEventHelper.TriggerEvents(changeReport);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new AbpDbConcurrencyException(ex.Message, ex);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var changeReport = ApplyAbpConcepts();
            var result = await base.SaveChangesAsync(cancellationToken);
            await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new AbpDbConcurrencyException(ex.Message, ex);
        }
    }

    public virtual void Initialize(AbpEfDbContextInitializationContext initializationContext)
    {
        var uowOptions = initializationContext.UnitOfWork.Options;
        if (uowOptions.Timeout.HasValue &&
            Database.IsRelational() &&
            !Database.GetCommandTimeout().HasValue)
        {
            Database.SetCommandTimeout(uowOptions.Timeout.Value.TotalSeconds.To<int>());
        }

        ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
    }

    protected virtual EntityChangeReport ApplyAbpConcepts()
    {
        var changeReport = new EntityChangeReport();

        var userId = GetAuditUserId();

        if (IsSoftDeleteCascadeEnabled)
        {
            CascadeSoftDelete();
        }

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            if (entry.State != EntityState.Modified && entry.CheckOwnedEntityChange())
            {
                Entry(entry.Entity).State = EntityState.Modified;
            }

            ApplyAbpConcepts(entry, userId, changeReport);
        }

        return changeReport;
    }

    protected virtual void ApplyAbpConcepts(EntityEntry entry, long? userId, EntityChangeReport changeReport)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                ApplyAbpConceptsForAddedEntity(entry, userId, changeReport);
                break;
            case EntityState.Modified:
                ApplyAbpConceptsForModifiedEntity(entry, userId, changeReport);
                break;
            case EntityState.Deleted:
                ApplyAbpConceptsForDeletedEntity(entry, userId, changeReport);
                break;
        }

        AddDomainEvents(changeReport.DomainEvents, entry.Entity);
    }

    protected virtual void ApplyAbpConceptsForAddedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
    {
        CheckAndSetId(entry);
        CheckAndSetMustHaveTenantIdProperty(entry.Entity);
        CheckAndSetMayHaveTenantIdProperty(entry.Entity);
        SetCreationAuditProperties(entry.Entity, userId);
        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
    }

    protected virtual void ApplyAbpConceptsForModifiedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
    {
        SetModificationAuditProperties(entry.Entity, userId);
        if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
        {
            SetDeletionAuditProperties(entry.Entity, userId);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }
        else
        {
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
        }
    }

    protected virtual void ApplyAbpConceptsForDeletedEntity(EntityEntry entry, long? userId, EntityChangeReport changeReport)
    {
        if (IsHardDeleteEntity(entry))
        {
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
            return;
        }

        CancelDeletionForSoftDelete(entry);
        SetDeletionAuditProperties(entry.Entity, userId);
        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
    }

    protected virtual bool IsHardDeleteEntity(EntityEntry entry)
    {
        if (!EntityHelper.IsEntity(entry.Entity.GetType()))
        {
            return false;
        }

        if (CurrentUnitOfWorkProvider?.Current?.Items == null)
        {
            return false;
        }

        if (!CurrentUnitOfWorkProvider.Current.Items.ContainsKey(UnitOfWorkExtensionDataTypes.HardDelete))
        {
            return false;
        }

        var hardDeleteItems = CurrentUnitOfWorkProvider.Current.Items[UnitOfWorkExtensionDataTypes.HardDelete];
        if (!(hardDeleteItems is HashSet<string> objects))
        {
            return false;
        }

        var currentTenantId = GetCurrentTenantIdOrNull();
        var hardDeleteKey = EntityHelper.GetHardDeleteKey(entry.Entity, currentTenantId);
        return objects.Contains(hardDeleteKey);
    }

    protected virtual void AddDomainEvents(List<DomainEventEntry> domainEvents, object entityAsObj)
    {
        var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
        if (generatesDomainEventsEntity == null)
        {
            return;
        }

        if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
        {
            return;
        }

        domainEvents.AddRange(generatesDomainEventsEntity.DomainEvents.Select(eventData => new DomainEventEntry(entityAsObj, eventData)));
        generatesDomainEventsEntity.DomainEvents.Clear();
    }

    protected virtual void CheckAndSetId(EntityEntry entry)
    {
        //Set GUID Ids
        var entity = entry.Entity as IEntity<Guid>;
        if (entity != null && entity.Id == Guid.Empty)
        {
            var idPropertyEntry = entry.Property("Id");

            if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
            {
                entity.Id = GuidGenerator.Create();
            }
        }
    }

    protected virtual void CheckAndSetMustHaveTenantIdProperty(object entityAsObj)
    {
        if (SuppressAutoSetTenantId)
        {
            return;
        }

        //Only set IMustHaveTenant entities
        if (!(entityAsObj is IMustHaveTenant))
        {
            return;
        }

        var entity = entityAsObj.As<IMustHaveTenant>();

        //Don't set if it's already set
        if (entity.TenantId != 0)
        {
            return;
        }

        var currentTenantId = GetCurrentTenantIdOrNull();

        if (currentTenantId != null)
        {
            entity.TenantId = currentTenantId.Value;
        }
        else
        {
            throw new AbpException("Can not set TenantId to 0 for IMustHaveTenant entities!");
        }
    }

    protected virtual void CheckAndSetMayHaveTenantIdProperty(object entityAsObj)
    {
        if (SuppressAutoSetTenantId)
        {
            return;
        }

        //Only works for single tenant applications
        if (MultiTenancyConfig?.IsEnabled ?? false)
        {
            return;
        }

        //Only set IMayHaveTenant entities
        if (!(entityAsObj is IMayHaveTenant))
        {
            return;
        }

        var entity = entityAsObj.As<IMayHaveTenant>();

        //Don't set if it's already set
        if (entity.TenantId != null)
        {
            return;
        }

        entity.TenantId = GetCurrentTenantIdOrNull();
    }

    protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
    {
        EntityAuditingHelper.SetCreationAuditProperties(
            MultiTenancyConfig,
            entityAsObj,
            AbpSession.TenantId,
            userId,
            CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
        );
    }

    protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
    {
        EntityAuditingHelper.SetModificationAuditProperties(
            MultiTenancyConfig,
            entityAsObj,
            AbpSession.TenantId,
            userId,
            CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
        );
    }

    /// <summary>
    /// BFS over every <see cref="ISoftDelete"/> Deleted entity in the change tracker, marking
    /// cascade-configured children as Deleted so the main <see cref="ApplyAbpConcepts()"/> loop
    /// can soft-delete (or hard-delete) them. Two corners are intentionally not rescued: an
    /// <see cref="ISoftDelete"/> grandchild reachable only through a non-soft middle node, and
    /// the children of a parent that goes through <c>IRepository.HardDelete</c>; in both cases
    /// EF Core's own OnSaveChanges cascade reasserts itself and hard-deletes the descendants.
    /// </summary>
    protected virtual void CascadeSoftDelete()
    {
        var queue = new Queue<EntityEntry>();
        SeedCascadeSoftDeleteQueue(queue);

        var visited = new HashSet<object>();

        while (queue.Count > 0)
        {
            var parentEntry = queue.Dequeue();
            if (!visited.Add(parentEntry.Entity))
            {
                continue;
            }

            foreach (var childEntry in GetCascadeSoftDeleteChildren(parentEntry))
            {
                if (TrySetCascadeSoftDeleteChildAsDeleted(childEntry))
                {
                    queue.Enqueue(childEntry);
                }
            }
        }
    }

    /// <summary>
    /// Picks the entries that should feed <see cref="CascadeSoftDelete"/>'s BFS. Override to add
    /// or skip seeds (e.g. extra entity types, custom hard-delete markers).
    /// </summary>
    protected virtual void SeedCascadeSoftDeleteQueue(Queue<EntityEntry> queue)
    {
        // Only ISoftDelete + non hard-delete parents need fixing up: they get rewritten to
        // Modified, so EF Core would skip the cascade entirely. Other Deleted parents issue a
        // real DELETE and EF Core handles their cascade itself.
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            if (entry.State == EntityState.Deleted &&
                entry.Entity is ISoftDelete &&
                !IsHardDeleteEntity(entry))
            {
                queue.Enqueue(entry);
            }
        }
    }

    /// <summary>
    /// Decides how a cascade child is taken over by the soft-delete pipeline and whether the BFS
    /// should keep walking through its own dependents. Returns <c>true</c> when the BFS should
    /// enqueue the child for further descent.
    /// </summary>
    protected virtual bool TrySetCascadeSoftDeleteChildAsDeleted(EntityEntry childEntry)
    {
        // Mirror EF Core's StateManager.CascadeDelete: an Added dependent is detached when the
        // principal is removed, so "added in this UoW, then orphaned by the cascade" disappears
        // instead of being persisted.
        if (childEntry.State == EntityState.Added)
        {
            childEntry.State = EntityState.Detached;
            return false;
        }

        if (childEntry.State == EntityState.Deleted ||
            childEntry.State == EntityState.Detached)
        {
            return false;
        }

        // Don't restamp DeletionTime / DeleterUserId on an already soft-deleted row.
        if (childEntry.Entity is ISoftDelete childSoftDelete && childSoftDelete.IsDeleted)
        {
            return false;
        }

        childEntry.State = EntityState.Deleted;

        // Only descend through soft-delete (non hard-delete) children: a non-soft child is about
        // to be DELETE'd for real and EF Core's own OnSaveChanges cascade pass will overwrite any
        // Modified state we set on its descendants anyway.
        return childEntry.Entity is ISoftDelete && !IsHardDeleteEntity(childEntry);
    }

    /// <summary>
    /// Returns the cascade children to walk for <paramref name="parentEntry"/>. Default walks
    /// the parent's principal-side navigations and loads them on demand; override to plug in
    /// a different lookup (e.g. foreign-key-based for entities with no navigation declared).
    /// </summary>
    protected virtual IEnumerable<EntityEntry> GetCascadeSoftDeleteChildren(EntityEntry parentEntry)
    {
        var children = new List<EntityEntry>();

        foreach (var navigationEntry in parentEntry.Navigations)
        {
            if (!ShouldFollowCascadeSoftDeleteNavigation(navigationEntry))
            {
                continue;
            }

            if (!navigationEntry.IsLoaded)
            {
                navigationEntry.Load();
            }

            CollectCascadeSoftDeleteChildren(navigationEntry, children);
        }

        return children;
    }

    /// <summary>
    /// Filter for the navigations that <see cref="GetCascadeSoftDeleteChildren"/> follows.
    /// Skips skip-navigations, owned navigations, dependent-side navigations and FKs whose
    /// <see cref="DeleteBehavior"/> is neither <see cref="DeleteBehavior.Cascade"/> nor
    /// <see cref="DeleteBehavior.ClientCascade"/>. Override to add custom filters.
    /// </summary>
    protected virtual bool ShouldFollowCascadeSoftDeleteNavigation(NavigationEntry navigationEntry)
    {
        // Skip skip-navigations (many-to-many): their join rows are handled by their own FKs.
        if (navigationEntry.Metadata is not INavigation navigation)
        {
            return false;
        }

        if (navigation.TargetEntityType.IsOwned())
        {
            return false;
        }

        if (navigation.IsOnDependent)
        {
            return false;
        }

        return navigation.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade ||
               navigation.ForeignKey.DeleteBehavior == DeleteBehavior.ClientCascade;
    }

    /// <summary>
    /// Pulls the tracked child entries out of a loaded <paramref name="navigationEntry"/> into
    /// <paramref name="children"/>. Override to reshape what counts as a cascade child for a
    /// given navigation.
    /// </summary>
    protected virtual void CollectCascadeSoftDeleteChildren(NavigationEntry navigationEntry, List<EntityEntry> children)
    {
        switch (navigationEntry)
        {
            case CollectionEntry collectionEntry when collectionEntry.CurrentValue != null:
                foreach (var child in collectionEntry.CurrentValue)
                {
                    children.Add(Entry(child));
                }

                break;
            case ReferenceEntry referenceEntry when referenceEntry.CurrentValue != null:
                children.Add(Entry(referenceEntry.CurrentValue));
                break;
        }
    }

    protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
    {
        if (!(entry.Entity is ISoftDelete))
        {
            return;
        }

        entry.Reload();
        entry.State = EntityState.Modified;
        entry.Entity.As<ISoftDelete>().IsDeleted = true;
    }

    protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
    {
        EntityAuditingHelper.SetDeletionAuditProperties(
            MultiTenancyConfig,
            entityAsObj,
            AbpSession.TenantId,
            userId,
            CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration
        );
    }

    protected virtual long? GetAuditUserId()
    {
        if (AbpSession.UserId.HasValue &&
            CurrentUnitOfWorkProvider != null &&
            CurrentUnitOfWorkProvider.Current != null &&
            CurrentUnitOfWorkProvider.Current.GetTenantId() == AbpSession.TenantId)
        {
            return AbpSession.UserId;
        }

        return null;
    }

    protected virtual int? GetCurrentTenantIdOrNull()
    {
        if (CurrentUnitOfWorkProvider != null &&
            CurrentUnitOfWorkProvider.Current != null)
        {
            return CurrentUnitOfWorkProvider.Current.GetTenantId();
        }

        return AbpSession.TenantId;
    }

    protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return ExpressionCombiner.Combine(expression1, expression2);
    }
}