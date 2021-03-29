using System;
using System.Data;
using System.Data.Entity;
using System.Reflection;
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

        private async Task<DbContext> GetDbContextAsync(ActiveTransactionProviderArgs args)
        {
            var dbContextProviderType = (Type)args["ContextType"];

            var method = typeof(EfActiveTransactionProvider)
                .GetMethod(nameof(EfActiveTransactionProvider.GetDbContextFromDbContextProviderAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dbContextProviderType);

            return await (Task<DbContext>) method.Invoke(this, new object[] {(MultiTenancySides?) args["MultiTenancySide"]});
        }

        private async Task<DbContext> GetDbContextFromDbContextProviderAsync<TDbContext>(MultiTenancySides multiTenancySides)
            where TDbContext : DbContext
        {
            using (var dbContextProviderWrapper = _iocResolver.ResolveAsDisposable(typeof(IDbContextProvider<TDbContext>)))
            {
                return await ((IDbContextProvider<TDbContext>) dbContextProviderWrapper.Object).GetDbContextAsync(multiTenancySides);
            }
        }
    }
}
