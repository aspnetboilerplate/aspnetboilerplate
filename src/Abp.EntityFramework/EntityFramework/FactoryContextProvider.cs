using System;
using System.Data.Entity;

namespace Abp.EntityFramework
{
    internal sealed class FactoryContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        private readonly Func<TDbContext> _dbContextFactory;

        public FactoryContextProvider(Func<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public TDbContext GetDbContext()
        {
            return _dbContextFactory();
        }
    }
}