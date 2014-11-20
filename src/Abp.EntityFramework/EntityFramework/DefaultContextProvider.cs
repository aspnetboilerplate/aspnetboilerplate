using System.Data.Entity;
using Abp.Domain.Uow;
using Abp.EntityFramework.Uow;
using Abp.Dependency;

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
            // TODO: ammachado: Remove this dependency
            return IocManager.Instance.Resolve<UnitOfWorkScope>().Current.GetDbContext<TDbContext>();
        }
    }
}