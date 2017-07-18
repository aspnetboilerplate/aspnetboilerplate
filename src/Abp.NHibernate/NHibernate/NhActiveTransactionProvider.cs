using System;
using System.Data;
using System.Reflection;
using Abp.Data;
using Abp.Dependency;
using Abp.Extensions;
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
            var dbTransaction = GetFieldValue(typeof(AdoTransaction), adoTransaction, "trans").As<IDbTransaction>();
            return dbTransaction;
        }

        public IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            return _sessionProvider.Session.Connection;
        }

        private static object GetFieldValue(Type type, object instance, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(instance);
        }
    }
}
