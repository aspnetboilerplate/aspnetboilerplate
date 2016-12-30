using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Transactions.Extensions;

namespace Abp.EntityFramework.Uow
{
    public class DbContextEfTransactionStrategy : IEfTransactionStrategy
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        public DbContextEfTransactionStrategy()
        {
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>();
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }


        public void Commit()
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Commit();
            }
        }

        public void InitDbContext(DbContext dbContext, string connectionString)
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
                activeTransaction.AttendedDbContexts.Add(dbContext);
            }

        }

        public void Dispose(IIocResolver iocResolver)
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                foreach (var attendedDbContext in activeTransaction.AttendedDbContexts)
                {
                    iocResolver.Release(attendedDbContext);
                }

                activeTransaction.DbContextTransaction.Dispose();
                activeTransaction.StarterDbContext.Dispose();
            }

            ActiveTransactions.Clear();
        }
    }
}