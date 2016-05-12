namespace Abp.EntityFramework
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString);
    }
}