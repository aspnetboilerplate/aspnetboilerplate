using System;
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

        [Obsolete("Use System.Linq.Queryable.Count() instead.")]
        int Count<T>(IQueryable<T> queryable);

        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable);

        [Obsolete("Use System.Linq.Queryable.ToList() instead.")]
        List<T> ToList<T>(IQueryable<T> queryable);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable);

        [Obsolete("Use System.Linq.Queryable.FirstOrDefault() instead.")]
        T FirstOrDefault<T>(IQueryable<T> queryable);

        Task<bool> AnyAsync<T>(IQueryable<T> queryable);
    }
}