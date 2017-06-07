using System.Data;
using System.Data.Common;

using Abp.Data;
using Abp.Domain.Entities;

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
                return new ActiveTransactionProviderArgs
                {
                    ["ContextType"] = typeof(TDbContext),
                    ["MultiTenancySide"] = MultiTenancySide
                };
            }
        }

        public override DbConnection Connection => (DbConnection)_activeTransactionProvider.GetActiveConnection(ActiveTransactionProviderArgs);

        public override DbTransaction ActiveTransaction => (DbTransaction)_activeTransactionProvider.GetActiveTransaction(ActiveTransactionProviderArgs);
    }
}
