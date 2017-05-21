using System.Data;

using Abp.Domain.Entities;
using Abp.Transactions;

namespace Abp.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : DapperRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>

    {
        private readonly IActiveTransactionProvider _activeTransactionProvider;

        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(activeTransactionProvider)
        {
            _activeTransactionProvider = activeTransactionProvider;
        }

        public ActiveTransactionProviderArgs ActiveTransactionProviderArgs
        {
            get
            {
                var args = new ActiveTransactionProviderArgs();
                args["ContextType"] = typeof(TDbContext);
                args["MultiTenancySide"] = MultiTenancySide;
                return args;
            }
        }

        public override IDbConnection Connection
        {
            get { return _activeTransactionProvider.GetActiveConnection(ActiveTransactionProviderArgs); }
        }

        public override IDbTransaction ActiveTransaction
        {
            get { return _activeTransactionProvider.GetActiveTransaction(ActiveTransactionProviderArgs); }
        }
    }
}
