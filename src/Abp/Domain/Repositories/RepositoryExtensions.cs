using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static async Task EnsureLoadedAsync<TEntity, TPrimaryKey, TProperty>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken = default(CancellationToken))
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            if (repository is ISupportsExplicitLoading<TEntity, TPrimaryKey>)
            {
                await (repository as ISupportsExplicitLoading<TEntity, TPrimaryKey>).EnsureLoadedAsync(entity, propertyExpression, cancellationToken);
            }
        }
    }
}
