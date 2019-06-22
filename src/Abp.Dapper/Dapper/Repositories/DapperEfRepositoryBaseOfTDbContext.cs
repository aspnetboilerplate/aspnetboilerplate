using System.Data.Common;
using Abp.Data;
using Abp.MultiTenancy;

namespace Abp.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext> : IDapperRepository
    {
        protected MultiTenancySides? MultiTenancySide { get; set; }

        private readonly IActiveTransactionProvider _activeTransactionProvider;

        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider)
        {
            _activeTransactionProvider = activeTransactionProvider;
        }

        protected virtual ActiveTransactionProviderArgs ActiveTransactionProviderArgs =>
            new ActiveTransactionProviderArgs
            {
                ["ContextType"] = typeof(TDbContext),
                ["MultiTenancySide"] = MultiTenancySide
            };

        protected virtual DbConnection Connection =>
            (DbConnection) _activeTransactionProvider.GetActiveConnection(ActiveTransactionProviderArgs);

        protected virtual DbTransaction ActiveTransaction =>
            (DbTransaction) _activeTransactionProvider.GetActiveTransaction(ActiveTransactionProviderArgs);
    }
}