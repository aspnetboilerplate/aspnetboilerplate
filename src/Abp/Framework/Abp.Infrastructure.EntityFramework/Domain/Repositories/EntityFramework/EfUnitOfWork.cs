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
        //public AbpDbContext Context { get; private set; }

        private IDictionary<Type, DbContext> _dbContexts;

        /// <summary>
        /// Reference to the currently running transcation.
        /// </summary>
        private TransactionScope _transaction;

        /// <summary>
        /// Creates a new instance of EfUnitOfWork.
        /// </summary>
        public EfUnitOfWork()
        {
            //Context = context;
        }

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                _dbContexts = new Dictionary<Type, DbContext>(); //TODO: Move to contrructor?
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
                _dbContexts.Values.ForEach(dbContext => dbContext.SaveChanges());
                
                //Context.SaveChanges();
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
            }

            _dbContexts.Values.ForEach(dbContext =>
                                       {
                                           dbContext.Dispose();
                                           IocHelper.Release(dbContext);
                                       });

            //if (Context != null)
            //{
            //    Context.Dispose();
            //}
        }

        internal TDbContext GetOrCreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            DbContext dbContext;
            if (!_dbContexts.TryGetValue(typeof(TDbContext), out dbContext))
            {
                _dbContexts[typeof(TDbContext)] = dbContext = IocHelper.Resolve<TDbContext>();
            }

            return (TDbContext)dbContext;
        }
    }
}