using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Abp.Dependency;
using Castle.Core.Internal;

namespace Abp.Domain.Uow.EntityFramework
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

        /// <summary>
        /// Is this object disposed?
        /// Used to prevent multiple dispose.
        /// </summary>
        private bool _disposed;

        public EfUnitOfWork()
        {
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
                                           IocHelper.Release(dbContext);
                                       });

            _activeDbContexts.Clear();
        }

        internal TDbContext GetOrCreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            DbContext dbContext;
            if (!_activeDbContexts.TryGetValue(typeof(TDbContext), out dbContext))
            {
                _activeDbContexts[typeof(TDbContext)] = dbContext = IocHelper.Resolve<TDbContext>();
            }

            return (TDbContext)dbContext;
        }
    }
}