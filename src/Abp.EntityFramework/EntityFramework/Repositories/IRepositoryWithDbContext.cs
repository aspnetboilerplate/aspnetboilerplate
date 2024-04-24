using System.Data.Entity;
using System.Threading.Tasks;

namespace Abp.EntityFramework.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
        Task<DbContext> GetDbContextAsync();
    }
}