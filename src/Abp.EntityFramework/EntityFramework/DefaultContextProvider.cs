using System.Data.Entity;
using Abp.Domain.Uow;
using Abp.EntityFramework.Uow;

namespace Abp.EntityFramework
{
    internal sealed class DefaultContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public static DefaultContextProvider<TDbContext> Instance { get { return _instance; } }
        private static readonly DefaultContextProvider<TDbContext> _instance = new DefaultContextProvider<TDbContext>();

        private DefaultContextProvider()
        {
            
        }

        public TDbContext GetDbContext()
        {
            return UnitOfWorkScope.Current.GetDbContext<TDbContext>();
        }
    }
}