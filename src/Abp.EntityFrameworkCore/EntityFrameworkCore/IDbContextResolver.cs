using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString)
            where TDbContext : DbContext;
    }
}