using System.Data.Common;

namespace Abp.EntityFramework
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString);

        TDbContext Resolve<TDbContext>(DbConnection existingConnection, bool contextOwnsConnection);
    }
}