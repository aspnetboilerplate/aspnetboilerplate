using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework.Utils;
using Abp.Extensions;
using Abp.MultiTenancy;
using Castle.Core.Internal;
using System;
using System.Linq;

namespace Abp.EntityFramework.Uow
{
    public class EfUnitOfWorkDbContextContainer
    {
        public DbContext DbContext { get; set; }
        public ObjectMaterializedEventHandler ObjectMaterializedDelegate { get; set; }
    }
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected IDictionary<string, EfUnitOfWorkDbContextContainer> ActiveDbContexts { get; }
        protected IIocResolver IocResolver { get; }

        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;
        private readonly IEfTransactionStrategy _transactionStrategy;

        /// <summary>
        /// Creates a new <see cref="EfUnitOfWork"/>.
        /// </summary>
        public EfUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver,
            IEfUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions,
            IDbContextTypeMatcher dbContextTypeMatcher,
            IEfTransactionStrategy transactionStrategy)
            : base(
                  connectionStringResolver,
                  defaultOptions,
                  filterExecuter)
        {
            IocResolver = iocResolver;
            _dbContextResolver = dbContextResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
            _transactionStrategy = transactionStrategy;

            ActiveDbContexts = new Dictionary<string, EfUnitOfWorkDbContextContainer>();
        }

        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.InitOptions(Options);
            }
        }

        public override void SaveChanges()
        {
            GetAllActiveDbContexts().ForEach(SaveChangesInDbContext);
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in GetAllActiveDbContexts())
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        public IReadOnlyList<DbContext> GetAllActiveDbContexts()
        {
            return ActiveDbContexts.Values.Select(x => x.DbContext).ToImmutableList();
        }

        protected override void CompleteUow()
        {
            SaveChanges();

            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Commit();
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();

            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Commit();
            }
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

            EfUnitOfWorkDbContextContainer dbContextContainer;
            if (!ActiveDbContexts.TryGetValue(dbContextKey, out dbContextContainer))
            {
                dbContextContainer = new EfUnitOfWorkDbContextContainer();
                if (Options.IsTransactional == true)
                {
                    dbContextContainer.DbContext = _transactionStrategy.CreateDbContext<TDbContext>(connectionString, _dbContextResolver);
                }
                else
                {
                    dbContextContainer.DbContext = _dbContextResolver.Resolve<TDbContext>(connectionString);
                }

                if (Options.Timeout.HasValue && !dbContextContainer.DbContext.Database.CommandTimeout.HasValue)
                {
                    dbContextContainer.DbContext.Database.CommandTimeout = Options.Timeout.Value.TotalSeconds.To<int>();
                }
                dbContextContainer.ObjectMaterializedDelegate = (sender, args) =>
                {
                    ObjectContext_ObjectMaterialized(dbContextContainer.DbContext, args);
                };
                ((IObjectContextAdapter)dbContextContainer.DbContext).ObjectContext.ObjectMaterialized += dbContextContainer.ObjectMaterializedDelegate;

                FilterExecuter.As<IEfUnitOfWorkFilterExecuter>().ApplyCurrentFilters(this, dbContextContainer.DbContext);
                
                ActiveDbContexts[dbContextKey] = dbContextContainer;
            }

            return (TDbContext)dbContextContainer.DbContext;
        }

        protected override void DisposeUow()
        {
            //detach our event handlers to avoid leaks
            foreach (var activeDbContext in ActiveDbContexts.Values)
            {
                if (activeDbContext.ObjectMaterializedDelegate != null)
                {
                    ((IObjectContextAdapter)activeDbContext.DbContext).ObjectContext.ObjectMaterialized -= activeDbContext.ObjectMaterializedDelegate;
                    activeDbContext.ObjectMaterializedDelegate = null;
                }
            }

            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Dispose(IocResolver);
            }
            else
            {
                foreach (var activeDbContext in ActiveDbContexts.Values)
                {
                    Release(activeDbContext);
                }
            }

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

        protected virtual void Release(EfUnitOfWorkDbContextContainer container)
        {
            container.DbContext.Dispose();
            IocResolver.Release(container.DbContext);
        }

        private static void ObjectContext_ObjectMaterialized(DbContext dbContext, ObjectMaterializedEventArgs e)
        {
            var entityType = ObjectContext.GetObjectType(e.Entity.GetType());

            dbContext.Configuration.AutoDetectChangesEnabled = false;
            var previousState = dbContext.Entry(e.Entity).State;

            DateTimePropertyInfoHelper.NormalizeDatePropertyKinds(e.Entity, entityType);

            dbContext.Entry(e.Entity).State = previousState;
            dbContext.Configuration.AutoDetectChangesEnabled = true;
        }
    }
}