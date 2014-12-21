using System.Data.Entity;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Uow
{
    public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
    {
        public TDbContext DbContext { get { return _currentUnitOfWorkProvider.Current.GetDbContext<TDbContext>(); } }

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public UnitOfWorkDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }
    }
}