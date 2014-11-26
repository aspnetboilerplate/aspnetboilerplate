using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Internal;

namespace Abp.EntityFramework.Uow
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class EfUnitOfWork : UnitOfWorkBase
    {
        /// <summary>
        /// List of DbContexes actively used in this unit of work.
        /// </summary>
        private readonly IDictionary<Type, DbContext> _activeDbContexts;

        /// <summary>
        /// Reference to the currently running transcation.
        /// </summary>
        private TransactionScope _transaction;

        private readonly IIocManager _iocManager;

        /// <summary>
        /// Is this object disposed?
        /// Used to prevent multiple dispose.
        /// </summary>
        private bool _disposed;

        public EfUnitOfWork(IIocManager iocManager)
        {
            _iocManager = iocManager;
            _activeDbContexts = new Dictionary<Type, DbContext>();
        }

        public override void Begin()
        {
            try
            {
                _activeDbContexts.Clear();
                var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
                if (IsTransactional)
                {
                    _transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions); //Required or RequiresNew?
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void SaveChanges()
        {
            _activeDbContexts.Values.ForEach(dbContext => dbContext.SaveChanges());
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in _activeDbContexts.Values)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public override void End()
        {
            try
            {
                SaveChanges();
                if (_transaction != null)
                {
                    _transaction.Complete();
                }

                TriggerSuccessHandlers();
            }
            finally
            {
                Dispose();
            }
        }

        public override async Task EndAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    _transaction.Complete();
                }

                TriggerSuccessHandlers();
            }
            finally
            {
                Dispose();
            }
        }

        public override void Cancel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            _activeDbContexts.Values.ForEach(dbContext =>
                                       {
                                           dbContext.Dispose();
                                           _iocManager.Release(dbContext);
                                       });

            _activeDbContexts.Clear();
        }

        internal TDbContext GetOrCreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            DbContext dbContext;
            if (!_activeDbContexts.TryGetValue(typeof(TDbContext), out dbContext))
            {
                _activeDbContexts[typeof(TDbContext)] = dbContext = _iocManager.Resolve<TDbContext>();
            }

            return (TDbContext)dbContext;
        }
    }
}