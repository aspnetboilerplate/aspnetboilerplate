namespace Abp.EntityFrameworkCore
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString);
    }
}