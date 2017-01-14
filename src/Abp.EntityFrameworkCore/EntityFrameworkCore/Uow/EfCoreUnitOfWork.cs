using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Extensions;
using Abp.MultiTenancy;
using Abp.Transactions.Extensions;
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

        protected IDbContextTransaction SharedTransaction;

        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        /// <summary>
        /// Creates a new <see cref="EfCoreUnitOfWork"/>.
        /// </summary>
        public EfCoreUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IDbContextResolver dbContextResolver,
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

            ActiveDbContexts = new Dictionary<string, DbContext>();
        }

        public override void SaveChanges()
        {
            foreach (var dbContext in ActiveDbContexts.Values)
            {
                SaveChangesInDbContext(dbContext);
            }
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
            CommitTransaction();
            DisposeUow(); //TODO: Is that needed?
        }

        private void CommitTransaction()
        {
            if (Options.IsTransactional != true)
            {
                return;
            }

            SharedTransaction?.Commit();

            foreach (var dbContext in ActiveDbContexts.Values)
            {
                if (dbContext.HasRelationalTransactionManager())
                {
                    //Relational databases use the SharedTransaction
                    continue;
                }

                dbContext.Database.CommitTransaction();
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            CommitTransaction();
            DisposeUow(); //TODO: Is that needed?
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

                //TODO: Object materialize event
                //TODO: Apply current filters to this dbcontext

                if (Options.IsTransactional == true)
                {
                    BeginTransaction(dbContext);
                }

                ActiveDbContexts[dbContextKey] = dbContext;
            }

            return (TDbContext)dbContext;
        }

        private void BeginTransaction(DbContext dbContext)
        {
            if (dbContext.HasRelationalTransactionManager())
            {
                if (SharedTransaction == null)
                {
                    SharedTransaction = dbContext.Database.BeginTransaction(
                        (Options.IsolationLevel ?? IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel()
                    );
                }
                else
                {
                    dbContext.Database.UseTransaction(SharedTransaction.GetDbTransaction());
                }
            }
            else
            {
                dbContext.Database.BeginTransaction();
            }
        }

        protected override void DisposeUow()
        {
            if (SharedTransaction != null)
            {
                SharedTransaction.Dispose();
                SharedTransaction = null;
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