using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Internal;

namespace Abp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class EfUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// List of DbContexes actively used in this unit of work.
        /// </summary>
        private IDictionary<Type, DbContext> _activeDbContexts;

        /// <summary>
        /// Reference to the currently running transcation.
        /// </summary>
        private TransactionScope _transaction;

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                _activeDbContexts = new Dictionary<Type, DbContext>(); //TODO: Move to contrructor?
                var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
                _transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        public void Commit()
        {
            try
            {
                _activeDbContexts.Values.ForEach(dbContext => dbContext.SaveChanges());
                _transaction.Complete();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        public void Rollback()
        {
            Dispose();
        }

        private void Dispose()
        {
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