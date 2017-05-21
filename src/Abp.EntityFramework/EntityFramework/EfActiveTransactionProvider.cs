using System;
using System.Data;
using System.Data.Entity;
using System.Reflection;

using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Transactions;

namespace Abp.EntityFramework
{
    public class EfActiveTransactionProvider : IActiveTransactionProvider, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public EfActiveTransactionProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            return GetDbContext(args).Database.CurrentTransaction.UnderlyingTransaction;
        }

        public IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            return GetDbContext(args).Database.Connection;
        }

        private DbContext GetDbContext(ActiveTransactionProviderArgs args)
        {
            var dbContextType = (Type)args["ContextType"];
            var multiTenancySide = (MultiTenancySides?)args["MultiTenancySide"];
            Type dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType(dbContextType);
            object dbContextProvider = _iocResolver.Resolve(dbContextProviderType);
            MethodInfo method = dbContextProvider.GetType().GetMethod(nameof(IDbContextProvider<AbpDbContext>.GetDbContext), new[] { typeof(MultiTenancySides) });
            var dbContext = method.Invoke(dbContextProvider, new object[] { multiTenancySide }).As<DbContext>();
            return dbContext;
        }
    }
}
