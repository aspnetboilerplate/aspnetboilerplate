using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Linq
{
    public class NullAsyncQueryableExecuter : IAsyncQueryableExecuter
    {
        public static NullAsyncQueryableExecuter Instance { get; } = new NullAsyncQueryableExecuter();

        public Task<int> CountAsync<T>(IQueryable<T> queryable)
        {
            return Task.FromResult(queryable.Count());
        }

        public int Count<T>(IQueryable<T> queryable)
        {
            return queryable.Count();
        }

        public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable)
        {
            return Task.FromResult(queryable.ToList());
        }

        public List<T> ToList<T>(IQueryable<T> queryable)
        {
            return queryable.ToList();
        }

        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable)
        {
            return Task.FromResult(queryable.FirstOrDefault());
        }

        public T FirstOrDefault<T>(IQueryable<T> queryable)
        {
            return queryable.FirstOrDefault();
        }
    }
}