using System;
using System.Data;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Abp.EntityFrameworkCore
{
    public class EfCoreActiveTransactionProvider : IActiveTransactionProvider, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public EfCoreActiveTransactionProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task<IDbTransaction> GetActiveTransactionAsync(ActiveTransactionProviderArgs args)
        {
            var context = await GetDbContextAsync(args);
            return context.Database.CurrentTransaction?.GetDbTransaction();
        }

        public IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            var context = GetDbContext(args);
            return context.Database.CurrentTransaction?.GetDbTransaction();
        }

        public async Task<IDbConnection> GetActiveConnectionAsync(ActiveTransactionProviderArgs args)
        {
            var context = await GetDbContextAsync(args);
            return context.Database.GetDbConnection();
        }

        public IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            var context = GetDbContext(args);
            return context.Database.GetDbConnection();
        }

        private async Task<DbContext> GetDbContextAsync(ActiveTransactionProviderArgs args)
        {
            var dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type) args["ContextType"]);

            using (IDisposableDependencyObjectWrapper dbContextProviderWrapper =
                _iocResolver.ResolveAsDisposable(dbContextProviderType))
            {
                var method = dbContextProviderWrapper.Object.GetType()
                    .GetMethod(
                        nameof(IDbContextProvider<AbpDbContext>.GetDbContextAsync),
                        new[] {typeof(MultiTenancySides)}
                    );
                
                var result = await ReflectionHelper.InvokeAsync(method, dbContextProviderWrapper.Object,
                    new object[] {(MultiTenancySides?) args["MultiTenancySide"]});
                
                return result as DbContext;
            }
        }

        private DbContext GetDbContext(ActiveTransactionProviderArgs args)
        {
            var dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type) args["ContextType"]);

            using (IDisposableDependencyObjectWrapper dbContextProviderWrapper =
                _iocResolver.ResolveAsDisposable(dbContextProviderType))
            {
                var method = dbContextProviderWrapper.Object.GetType()
                    .GetMethod(
                        nameof(IDbContextProvider<AbpDbContext>.GetDbContext),
                        new[] {typeof(MultiTenancySides)}
                    );

                return (DbContext) method.Invoke(
                    dbContextProviderWrapper.Object,
                    new object[] {(MultiTenancySides?) args["MultiTenancySide"]}
                );
            }
        }
    }
}
