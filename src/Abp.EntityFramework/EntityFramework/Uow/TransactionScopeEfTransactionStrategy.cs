using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Uow
{
    public class TransactionScopeEfTransactionStrategy : IEfTransactionStrategy, ITransientDependency
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected TransactionScope CurrentTransaction { get; set; }

        protected List<DbContext> DbContexts { get; }

        public TransactionScopeEfTransactionStrategy()
        {
            DbContexts = new List<DbContext>();
        }

        public virtual void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }

        public virtual void Commit()
        {
            CurrentTransaction?.Complete();
        }

        public virtual void InitDbContext(DbContext dbContext, string connectionString)
        {
            EnsureCurrentTransactionInitialized();
            DbContexts.Add(dbContext);
        }

        private void EnsureCurrentTransactionInitialized()
        {
            if (CurrentTransaction != null)
            {
                return;
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = Options.IsolationLevel.GetValueOrDefault(IsolationLevel.ReadUncommitted),
            };

            if (Options.Timeout.HasValue)
            {
                transactionOptions.Timeout = Options.Timeout.Value;
            }

            //TODO: LAZY!
            CurrentTransaction = new TransactionScope(
                Options.Scope.GetValueOrDefault(TransactionScopeOption.Required),
                transactionOptions,
                Options.AsyncFlowOption.GetValueOrDefault(TransactionScopeAsyncFlowOption.Enabled)
            );
        }

        public virtual void Dispose(IIocResolver iocResolver)
        {
            foreach (var dbContext in DbContexts)
            {
                iocResolver.Release(dbContext);
            }

            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }
}