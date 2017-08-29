using Abp.Dependency;
using Abp.Domain.Entities;

namespace Abp.Dapper.Filters.Action
{
    public class DapperActionFilterExecuter : IDapperActionFilterExecuter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public DapperActionFilterExecuter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            _iocResolver.Resolve<CreationAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            _iocResolver.Resolve<ModificationAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }

        public void ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            _iocResolver.Resolve<DeletionAuditDapperActionFilter>().ExecuteFilter<TEntity, TPrimaryKey>(entity);
        }
    }
}
