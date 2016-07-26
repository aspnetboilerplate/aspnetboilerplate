using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.MultiTenancy;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Abp.EntityFrameworkCore.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfCoreUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected IDictionary<string, DbContext> ActiveDbContexts { get; private set; }

        protected IIocResolver IocResolver { get; private set; }

        protected IDbContextTransaction CurrentTransaction;

        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        /// <summary>
        /// Creates a new <see cref="EfCoreUnitOfWork"/>.
        /// </summary>
        public EfCoreUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver,
            IUnitOfWorkDefaultOptions defaultOptions, 
            IDbContextTypeMatcher dbContextTypeMatcher)
            : base(connectionStringResolver, defaultOptions)
        {
            IocResolver = iocResolver;
            _dbContextResolver = dbContextResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;

            ActiveDbContexts = new Dictionary<string, DbContext>();
        }

        protected override void BeginUow()
        {
            //if (Options.IsTransactional == true)
            //{
            //    var transactionOptions = new TransactionOptions
            //    {
            //        IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
            //    };

            //    if (Options.Timeout.HasValue)
            //    {
            //        transactionOptions.Timeout = Options.Timeout.Value;
            //    }

            //    CurrentTransaction = new TransactionScope(
            //        Options.Scope.GetValueOrDefault(TransactionScopeOption.Required),
            //        transactionOptions,
            //        Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
            //        );
            //}
        }

        public override void SaveChanges()
        {
            ActiveDbContexts.Values.ForEach(SaveChangesInDbContext);
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in ActiveDbContexts.Values)
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        protected override void CompleteUow()
        {
            SaveChanges();
            CurrentTransaction?.Commit();
            DisposeUow(); //TODO: Is that needed?
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            CurrentTransaction?.Commit();
            DisposeUow(); //TODO: Is that needed?
        }

        protected override void ApplyDisableFilter(string filterName)
        {
            //Disabled since there is no implementation.
            //foreach (var activeDbContext in ActiveDbContexts.Values)
            //{
            //    activeDbContext.DisableFilter(filterName);
            //}
        }

        protected override void ApplyEnableFilter(string filterName)
        {
            //Disabled since there is no implementation.
            //foreach (var activeDbContext in ActiveDbContexts.Values)
            //{
            //    activeDbContext.EnableFilter(filterName);
            //}
        }

        //Disabled since there is no implementation.
        protected override void ApplyFilterParameterValue(string filterName, string parameterName, object value)
        {
            //foreach (var activeDbContext in ActiveDbContexts.Values)
            //{
            //    if (TypeHelper.IsFunc<object>(value))
            //    {
            //        activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, (Func<object>)value);
            //    }
            //    else
            //    {
            //        activeDbContext.SetFilterScopedParameterValue(filterName, parameterName, value);
            //    }
            //}
        }

        public virtual TDbContext GetOrCreateDbContext<TDbContext>(MultiTenancySides? multiTenancySide = null)
            where TDbContext : DbContext
        {
            var concreteDbContextType = _dbContextTypeMatcher.GetConcreteType(typeof(TDbContext));

            var connectionStringResolveArgs = new ConnectionStringResolveArgs(multiTenancySide);
            connectionStringResolveArgs["DbContextType"] = typeof(TDbContext);
            connectionStringResolveArgs["DbContextConcreteType"] = concreteDbContextType;
            var connectionString = ResolveConnectionString(connectionStringResolveArgs);

            var dbContextKey = concreteDbContextType.FullName + "#" + connectionString;

            DbContext dbContext;
            if (!ActiveDbContexts.TryGetValue(dbContextKey, out dbContext))
            {

                dbContext = _dbContextResolver.Resolve<TDbContext>(connectionString);
                //((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                //{
                //    ObjectContext_ObjectMaterialized(dbContext, args);
                //};

                //foreach (var filter in Filters)
                //{
                //    if (filter.IsEnabled)
                //    {
                //        dbContext.EnableFilter(filter.FilterName);
                //    }
                //    else
                //    {
                //        dbContext.DisableFilter(filter.FilterName);
                //    }

                //    foreach (var filterParameter in filter.FilterParameters)
                //    {
                //        if (TypeHelper.IsFunc<object>(filterParameter.Value))
                //        {
                //            dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, (Func<object>)filterParameter.Value);
                //        }
                //        else
                //        {
                //            dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, filterParameter.Value);
                //        }
                //    }
                //}

                if (Options.IsTransactional == true)
                {
                    if (CurrentTransaction == null)
                    {
                        CurrentTransaction = dbContext.Database.BeginTransaction();
                    }
                }

                ActiveDbContexts[dbContextKey] = dbContext;
            }

            return (TDbContext)dbContext;
        }

        protected override void DisposeUow()
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }

            ActiveDbContexts.Values.ForEach(Release);
            ActiveDbContexts.Clear();
        }

        protected virtual void SaveChangesInDbContext(DbContext dbContext)
        {
            dbContext.SaveChanges();
        }

        protected virtual async Task SaveChangesInDbContextAsync(DbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }

        protected virtual void Release(DbContext dbContext)
        {
            dbContext.Dispose();
            IocResolver.Release(dbContext);
        }

        //private static void ObjectContext_ObjectMaterialized(DbContext dbContext, ObjectMaterializedEventArgs e)
        //{
        //    var entityType = ObjectContext.GetObjectType(e.Entity.GetType());

        //    dbContext.Configuration.AutoDetectChangesEnabled = false;
        //    var previousState = dbContext.Entry(e.Entity).State;

        //    DateTimePropertyInfoHelper.NormalizeDatePropertyKinds(e.Entity, entityType);

        //    dbContext.Entry(e.Entity).State = previousState;
        //    dbContext.Configuration.AutoDetectChangesEnabled = true;
        //}
    }
}