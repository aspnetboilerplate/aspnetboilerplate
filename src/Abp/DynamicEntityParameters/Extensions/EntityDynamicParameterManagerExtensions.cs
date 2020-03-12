using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters.Extensions
{
    public static class EntityDynamicParameterManagerExtensions
    {
        public static List<EntityDynamicParameter> GetAll<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAll(entityFullName: typeof(TEntity).FullName);
        }

        public static List<EntityDynamicParameter> GetAll<TEntity>(this IEntityDynamicParameterManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAll<TEntity, int>();
        }

        public static Task<List<EntityDynamicParameter>> GetAllAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAllAsync(entityFullName: typeof(TEntity).FullName);
        }

        public static Task<List<EntityDynamicParameter>> GetAllAsync<TEntity>(this IEntityDynamicParameterManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAllAsync<TEntity, int>();
        }

        public static EntityDynamicParameter Add<TEntity>(this IEntityDynamicParameterManager manager, int dynamicParameterId, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity, int>(dynamicParameterId, tenantId);
        }

        public static EntityDynamicParameter Add<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager, int dynamicParameterId, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            var entity = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameterId,
                EntityFullName = typeof(TEntity).FullName,
                TenantId = tenantId
            };
            manager.Add(entity);
            return entity;
        }

        public static Task<EntityDynamicParameter> AddAsync<TEntity>(this IEntityDynamicParameterManager manager, int dynamicParameterId, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity, int>(dynamicParameterId, tenantId);
        }

        public static async Task<EntityDynamicParameter> AddAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager, int dynamicParameterId, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            var entity = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameterId,
                EntityFullName = typeof(TEntity).FullName,
                TenantId = tenantId
            };
            await manager.AddAsync(entity);
            return entity;
        }

        public static EntityDynamicParameter Add<TEntity>(this IEntityDynamicParameterManager manager, DynamicParameter dynamicParameter, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity>(dynamicParameter.Id, tenantId);
        }

        public static EntityDynamicParameter Add<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager, DynamicParameter dynamicParameter, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.Add<TEntity, TPrimaryKey>(dynamicParameter.Id, tenantId);
        }

        public static Task<EntityDynamicParameter> AddAsync<TEntity>(this IEntityDynamicParameterManager manager, DynamicParameter dynamicParameter, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity>(dynamicParameter.Id, tenantId);
        }

        public static Task<EntityDynamicParameter> AddAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterManager manager, DynamicParameter dynamicParameter, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.AddAsync<TEntity, TPrimaryKey>(dynamicParameter.Id, tenantId);
        }
    }
}
