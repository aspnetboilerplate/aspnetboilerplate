using System.Collections.Generic;
using System.Linq;

using Abp.Dependency;
using Abp.Domain.Entities;

namespace Abp.Dapper.Filters.Action
{
    public class DapperActionFilterExecuter : IDapperActionFilterExecuter, ITransientDependency
    {
        private readonly IEnumerable<IDapperActionFilter> _actionFilters;

        public DapperActionFilterExecuter(IIocResolver iocResolver)
        {
            _actionFilters = iocResolver.ResolveAll<IDapperActionFilter>();
        }

        public void ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            IDapperActionFilter filter = _actionFilters.First(x => x.GetType() == typeof(CreationAuditDapperActionFilter));
            filter.ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            IDapperActionFilter filter = _actionFilters.First(x => x.GetType() == typeof(ModificationAuditDapperActionFilter));
            filter.ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            IDapperActionFilter filter = _actionFilters.First(x => x.GetType() == typeof(DeletionAuditDapperActionFilter));
            filter.ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }
    }
}
