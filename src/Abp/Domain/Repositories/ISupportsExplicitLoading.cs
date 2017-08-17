using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    public interface ISupportsExplicitLoading<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        Task EnsureLoadedAsync<TProperty>(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken)
            where TProperty : class;
    }
}