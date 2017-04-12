using Abp.Dependency;
using Abp.Domain.Entities;

namespace Abp.Dapper.Filters.Action
{
    public interface IDapperActionFilter : ITransientDependency
    {
        void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
