using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Extensions;
using Abp.Transactions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Transactions;

namespace Abp.EntityFrameworkCore.Uow
{
    public class DbContextEfCoreTransactionStrategy : IEfCoreTransactionStrategy, ITransientDependency
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        public DbContextEfCoreTransactionStrategy()
        {
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>();
        }

        public void Commit()
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Commit();

                foreach (var dbContext in activeTransaction.AttendedDbContexts)
                {
                    if (dbContext.HasRelationalTransactionManager())
                    {
                        //Relational databases use the SharedTransaction
                        continue;
                    }

                    dbContext.Database.CommitTransaction();
                }
            }
        }

        public DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver) where TDbContext : DbContext
        {
            DbContext dbContext;

            var activeTransaction = ActiveTransactions.GetOrDefault(connectionString);

            if (activeTransaction == null)
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(connectionString);
            }
            else
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(activeTransaction.DbContextTransaction.GetDbTransaction().Connection);
            }

            if (dbContext.HasRelationalTransactionManager())
            {
                if (activeTransaction == null)
                {
                    var dbtransaction = dbContext.Database.BeginTransaction((Options.IsolationLevel ?? IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel());
                    activeTransaction = new ActiveTransactionInfo(dbtransaction, dbContext);
                    ActiveTransactions[connectionString] = activeTransaction;
                }
                else
                {
                    dbContext.Database.UseTransaction(activeTransaction.DbContextTransaction.GetDbTransaction());
                    activeTransaction.AttendedDbContexts.Add(dbContext);
                }
            }
            else
            {
                dbContext.Database.BeginTransaction();
            }

            return dbContext;
        }

        public void Dispose(IIocResolver iocResolver)
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Dispose();

                foreach (var attendedDbContext in activeTransaction.AttendedDbContexts)
                {
                    iocResolver.Release(attendedDbContext);
                }
                                
                activeTransaction.StarterDbContext.Dispose();
            }

            ActiveTransactions.Clear();
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }
    }
}
