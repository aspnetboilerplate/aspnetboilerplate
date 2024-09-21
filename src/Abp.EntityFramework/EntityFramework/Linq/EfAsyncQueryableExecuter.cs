using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Linq;
using Abp.Threading;

namespace Abp.EntityFramework.Linq
{
    public class EfAsyncQueryableExecuter : IAsyncQueryableExecuter, ISingletonDependency
    {
        private readonly ICancellationTokenProvider _cancellationTokenProvider;

        public EfAsyncQueryableExecuter(ICancellationTokenProvider cancellationTokenProvider)
        {
            _cancellationTokenProvider = cancellationTokenProvider;
        }

        public Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(queryable, (q, token) => q.CountAsync(token), cancellationToken);
        }

        public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(queryable, (q, token) => q.ToListAsync(token), cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(queryable, (q, token) => q.FirstOrDefaultAsync(token), cancellationToken);
        }

        public Task<bool> AnyAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(queryable, (q, token) => q.AnyAsync(token), cancellationToken);
        }

        private async Task<TResult> ExecuteAsync<T, TResult>(IQueryable<T> queryable,
            Func<IQueryable<T>, CancellationToken, Task<TResult>> executeMethod,
            CancellationToken cancellationToken = default)
        {
            cancellationToken = _cancellationTokenProvider.FallbackToProvider(cancellationToken);
            return await executeMethod(queryable, cancellationToken).ConfigureAwait(false);
        }
    }
}
