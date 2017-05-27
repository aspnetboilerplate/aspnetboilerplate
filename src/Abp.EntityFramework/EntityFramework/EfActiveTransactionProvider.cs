using System;
using System.Data;
using System.Data.Entity;
using Abp.Data;
using Abp.Dependency;
using Abp.MultiTenancy;

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
            var dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type)args["ContextType"]);

            using (var dbContextProviderWrapper = _iocResolver.ResolveAsDisposable(dbContextProviderType))
            {
                var method = dbContextProviderWrapper.Object.GetType()
                    .GetMethod(
                        nameof(IDbContextProvider<AbpDbContext>.GetDbContext),
                        new[] {typeof(MultiTenancySides)}
                    );

                return (DbContext) method.Invoke(
                    dbContextProviderWrapper.Object,
                    new object[] { (MultiTenancySides?)args["MultiTenancySide"] }
                );
            }
        }
    }
}
