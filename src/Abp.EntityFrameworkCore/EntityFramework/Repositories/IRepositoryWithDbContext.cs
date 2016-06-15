using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFramework.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}