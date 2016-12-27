using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework.Utils;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Transactions.Extensions;
using Castle.Core.Internal;

namespace Abp.EntityFramework.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected IDictionary<string, ActiveDbContextInfo> ActiveDbContexts { get; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        protected IIocResolver IocResolver { get; }

        private readonly IDbContextResolver _dbContextResolver;

        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        /// <summary>
        /// Creates a new <see cref="EfUnitOfWork"/>.
        /// </summary>
        public EfUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver,
            IEfUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions, 
            IDbContextTypeMatcher dbContextTypeMatcher)
            : base(
                  connectionStringResolver, 
                  defaultOptions,
                  filterExecuter)
        {
            IocResolver = iocResolver;
            _dbContextResolver = dbContextResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;

            ActiveDbContexts = new Dictionary<string, ActiveDbContextInfo>();
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>();
        }

        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
                };

                if (Options.Timeout.HasValue)
                {
                    transactionOptions.Timeout = Options.Timeout.Value;
                }

                //CurrentTransaction = new TransactionScope(
                //    Options.Scope.GetValueOrDefault(TransactionScopeOption.Required),
                //    transactionOptions,
                //    Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
                //    );
            }
        }

        public override void SaveChanges()
        {
            ActiveDbContexts.Values.Select(c => c.DbContext).ForEach(SaveChangesInDbContext);
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in ActiveDbContexts.Values.Select(c => c.DbContext))
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        public IReadOnlyList<DbContext> GetAllActiveDbContexts()
        {
            return ActiveDbContexts.Values.Select(c => c.DbContext).ToImmutableList();
        }

        protected override void CompleteUow()
        {
            SaveChanges();
            CommitTransactions();
            DisposeUow();
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            CommitTransactions();
            DisposeUow();
        }

        private void CommitTransactions()
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Commit();
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

            ActiveDbContextInfo dbContextInfo;
            if (!ActiveDbContexts.TryGetValue(dbContextKey, out dbContextInfo))
            {
                var dbContext = _dbContextResolver.Resolve<TDbContext>(connectionString);

                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    ObjectContext_ObjectMaterialized(dbContext, args);
                };

                FilterExecuter.As<IEfUnitOfWorkFilterExecuter>().ApplyCurrentFilters(this, dbContext);

                if (Options.IsTransactional == true)
                {
                    InitTransaction(dbContext, connectionString);
                }

                ActiveDbContexts[dbContextKey] = dbContextInfo = new ActiveDbContextInfo(dbContext);
            }

            return (TDbContext)dbContextInfo.DbContext;
        }

        private void InitTransaction(DbContext dbContext, string connectionString)
        {
            var activeTransaction = ActiveTransactions.GetOrDefault(connectionString);
            if (activeTransaction == null)
            {
                activeTransaction = new ActiveTransactionInfo(
                    dbContext.Database.BeginTransaction(
                        (Options.IsolationLevel ?? IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel()
                    ),
                    dbContext
                );

                ActiveTransactions[connectionString] = activeTransaction;
            }
            else
            {
                dbContext.Database.UseTransaction(activeTransaction.DbContextTransaction.UnderlyingTransaction);
                activeTransaction.AttendeDbContexts.Add(dbContext);
            }
        }

        protected override void DisposeUow()
        {
            //TODO: Handle exceptions?

            if (Options.IsTransactional == true)
            {
                foreach (var activeTransaction in ActiveTransactions.Values)
                {
                    foreach (var attendeDbContext in activeTransaction.AttendeDbContexts)
                    {
                        Release(attendeDbContext);
                    }

                    activeTransaction.DbContextTransaction.Dispose();
                    activeTransaction.StarterDbContext.Dispose();
                }
            }
            else
            {
                foreach (var activeDbContext in ActiveDbContexts.Values)
                {
                    Release(activeDbContext.DbContext);
                }
            }

            ActiveTransactions.Clear();
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