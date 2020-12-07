using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
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

        public async Task<IDbTransaction> GetActiveTransactionAsync(ActiveTransactionProviderArgs args)
        {
            var context = await GetDbContextAsync(args);
            return context.Database.CurrentTransaction.UnderlyingTransaction;
        }

        public IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            return GetDbContext(args).Database.CurrentTransaction.UnderlyingTransaction;
        }

        public async Task<IDbConnection> GetActiveConnectionAsync(ActiveTransactionProviderArgs args)
        {
            var context = await GetDbContextAsync(args);
            return context.Database.Connection;
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
        
        private Task<DbContext> GetDbContextAsync(ActiveTransactionProviderArgs args)
        {
            var dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type)args["ContextType"]);

            using (var dbContextProviderWrapper = _iocResolver.ResolveAsDisposable(dbContextProviderType))
            {
                var method = dbContextProviderWrapper.Object.GetType()
                    .GetMethod(
                        nameof(IDbContextProvider<AbpDbContext>.GetDbContextAsync),
                        new[] {typeof(MultiTenancySides)}
                    );
                
                return (Task<DbContext>) method.Invoke(
                    dbContextProviderWrapper.Object,
                    new object[] { (MultiTenancySides?)args["MultiTenancySide"] }
                );
            }
        }
    }
}
