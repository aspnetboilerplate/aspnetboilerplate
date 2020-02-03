using System.Data;
using System.Data.Common;

using Abp.Data;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Extensions;

namespace Abp.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : DapperRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>

    {
        private readonly IActiveTransactionProvider _activeTransactionProvider;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(activeTransactionProvider)
        {
            _activeTransactionProvider = activeTransactionProvider;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
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

        public override int? Timeout => _currentUnitOfWorkProvider.Current?.Options.Timeout?.TotalSeconds.To<int>();
    }
}
