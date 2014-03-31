using System.Data.Entity;
using System.Transactions;
using Abp.Domain.Uow;

namespace Abp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// Implements Unit of work for NHibernate.
    /// </summary>
    public class EfUnitOfWork : IUnitOfWork
    {
        public AbpDbContext Context { get; private set; }

        /// <summary>
        /// Reference to the currently running transcation.
        /// </summary>
        private TransactionScope _transaction;

        /// <summary>
        /// Creates a new instance of EfUnitOfWork.
        /// </summary>
        /// <param name="context"></param>
        public EfUnitOfWork(AbpDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
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
                Context.SaveChanges();
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

            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}