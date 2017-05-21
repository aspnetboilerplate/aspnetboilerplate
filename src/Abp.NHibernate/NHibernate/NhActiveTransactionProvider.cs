using System.Data;

using Abp.Dependency;
using Abp.Extensions;
using Abp.Reflection;
using Abp.Transactions;

using NHibernate.Transaction;

namespace Abp.NHibernate
{
    public class NhActiveTransactionProvider : IActiveTransactionProvider, ITransientDependency
    {
        private readonly ISessionProvider _sessionProvider;

        public NhActiveTransactionProvider(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            var adoTransaction = _sessionProvider.Session.Transaction.As<AdoTransaction>();
            var dbTransaction = TypeHelper.GetInstanceField(typeof(AdoTransaction), adoTransaction, "trans").As<IDbTransaction>();
            return dbTransaction;
        }

        public IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            return _sessionProvider.Session.Connection;
        }
    }
}
