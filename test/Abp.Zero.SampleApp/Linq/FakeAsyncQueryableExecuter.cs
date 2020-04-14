using Abp.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Zero.SampleApp.Linq
{
    /// <summary>
    /// <see cref="FakeAsyncQueryableExecuter"/> <see langword="await"/>s an asynchronous <see cref="Task"/> before executing an operation synchronously.
    /// This differs from <see cref="NullAsyncQueryableExecuter"/> a.k.a. SyncQueryableExecuter, which actually only executes an operation synchronously.
    /// This can be used with tests to catch code that does not properly <see langword="await"/> a <see cref="Task"/> in a <see langword="using"/> block.
    /// </summary>
    public class FakeAsyncQueryableExecuter : IAsyncQueryableExecuter
    {
        public async Task<bool> AnyAsync<T>(IQueryable<T> queryable)
        {
            await AsyncTask();
            return queryable.Any();
        }

        public int Count<T>(IQueryable<T> queryable)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> CountAsync<T>(IQueryable<T> queryable)
        {
            await AsyncTask();
            return queryable.Count();
        }

        public T FirstOrDefault<T>(IQueryable<T> queryable)
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable)
        {
            await AsyncTask();
            return queryable.FirstOrDefault();
        }

        public List<T> ToList<T>(IQueryable<T> queryable)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<T>> ToListAsync<T>(IQueryable<T> queryable)
        {
            await AsyncTask();
            return queryable.ToList();
        }

        private Task AsyncTask()
        {
            return Task.Delay(1); // Task.Delay(0) and Task.CompletedTask are synchronous
        }
    }
}
