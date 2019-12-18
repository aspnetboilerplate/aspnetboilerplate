using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Linq
{
    /// <summary>
    /// This interface is intended to be used by ABP.
    /// </summary>
    public interface IAsyncQueryableExecuter
    {
        Task<int> CountAsync<T>(IQueryable<T> queryable);

        int Count<T>(IQueryable<T> queryable);

        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable);

        List<T> ToList<T>(IQueryable<T> queryable);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable);

        T FirstOrDefault<T>(IQueryable<T> queryable);
    }
}