using System;
using System.Linq.Expressions;

using Abp.Dependency;
using Abp.Domain.Entities;

using DapperExtensions;

namespace Abp.Dapper.Filters.Query
{
    public interface IDapperQueryFilter : ITransientDependency
    {
        string FilterName { get; }

        bool IsEnabled { get; }

        IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;

        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
