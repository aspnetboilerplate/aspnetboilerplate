using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
        
        Task<DbContext> GetDbContextAsync();
    }
}
