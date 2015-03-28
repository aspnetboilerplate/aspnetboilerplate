using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Internal;
using EntityFramework.DynamicFilters;

namespace Abp.EntityFramework.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IDictionary<Type, DbContext> _activeDbContexts;
        private readonly List<string> _disabledFilters;
        private readonly IIocResolver _iocResolver;
        private TransactionScope _transaction;

        /// <summary>
        /// Creates a new <see cref="EfUnitOfWork"/>.
        /// </summary>
        public EfUnitOfWork(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _activeDbContexts = new Dictionary<Type, DbContext>();
            _disabledFilters = new List<string>();
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

                _transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    transactionOptions,
                    Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
                    );
            }
        }

        public override void SaveChanges()
        {
            _activeDbContexts.Values.ForEach(SaveChangesInDbContext);
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in _activeDbContexts.Values)
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        protected override void CompleteUow()
        {
            SaveChanges();
            if (_transaction != null)
            {
                _transaction.Complete();
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                _transaction.Complete();
            }
        }

        public override void DisableFilter(string filterName)
        {
            if (_disabledFilters.Contains(filterName))
            {
                return;
            }

            _disabledFilters.Add(filterName);
            foreach (var activeDbContext in _activeDbContexts.Values)
            {
                activeDbContext.DisableFilter(filterName);
            }
        }

        internal TDbContext GetOrCreateDbContext<TDbContext>()
            where TDbContext : DbContext
        {
            DbContext dbContext;
            if (!_activeDbContexts.TryGetValue(typeof(TDbContext), out dbContext))
            {
                dbContext = _iocResolver.Resolve<TDbContext>();

                foreach (var disabledFilter in _disabledFilters)
                {
                    dbContext.DisableFilter(disabledFilter);
                }

                _activeDbContexts[typeof(TDbContext)] = dbContext;
            }

            return (TDbContext)dbContext;
        }

        protected override void DisposeUow()
        {
            _activeDbContexts.Values.ForEach(dbContext =>
            {
                dbContext.Dispose();
                _iocResolver.Release(dbContext);
            });

            if (_transaction != null)
            {
                _transaction.Dispose();
            }
        }

        protected virtual void SaveChangesInDbContext(DbContext dbContext)
        {
            dbContext.SaveChanges();
        }

        protected virtual async Task SaveChangesInDbContextAsync(DbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}