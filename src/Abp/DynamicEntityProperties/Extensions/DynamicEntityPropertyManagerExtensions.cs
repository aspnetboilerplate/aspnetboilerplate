using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityProperties.Extensions
{
    public static class DynamicEntityPropertyManagerExtensions
    {
        public static List<DynamicEntityProperty> GetAll<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAll(entityFullName: typeof(TEntity).FullName);
        }

        public static List<DynamicEntityProperty> GetAll<TEntity>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAll<TEntity, int>();
        }

        public static Task<List<DynamicEntityProperty>> GetAllAsync<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAllAsync(entityFullName: typeof(TEntity).FullName);
        }

        public static Task<List<DynamicEntityProperty>> GetAllAsync<TEntity>(this IDynamicEntityPropertyManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAllAsync<TEntity, int>();
        }

        public static DynamicEntityProperty Add<TEntity>(this IDynamicEntityPropertyManager manager, int dynamicPropertyId, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity, int>(dynamicPropertyId, tenantId);
        }

        public static DynamicEntityProperty Add<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager, int dynamicPropertyId, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            var entity = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicPropertyId,
                EntityFullName = typeof(TEntity).FullName,
                TenantId = tenantId
            };
            manager.Add(entity);
            return entity;
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity>(this IDynamicEntityPropertyManager manager, int dynamicPropertyId, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity, int>(dynamicPropertyId, tenantId);
        }

        public static async Task<DynamicEntityProperty> AddAsync<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager, int dynamicPropertyId, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            var entity = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicPropertyId,
                EntityFullName = typeof(TEntity).FullName,
                TenantId = tenantId
            };
            await manager.AddAsync(entity);
            return entity;
        }

        public static DynamicEntityProperty Add<TEntity>(this IDynamicEntityPropertyManager manager, DynamicProperty dynamicProperty, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.Add<TEntity>(dynamicProperty.Id, tenantId);
        }

        public static DynamicEntityProperty Add<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager, DynamicProperty dynamicProperty, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.Add<TEntity, TPrimaryKey>(dynamicProperty.Id, tenantId);
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity>(this IDynamicEntityPropertyManager manager, DynamicProperty dynamicProperty, int? tenantId)
            where TEntity : IEntity<int>
        {
            return manager.AddAsync<TEntity>(dynamicProperty.Id, tenantId);
        }

        public static Task<DynamicEntityProperty> AddAsync<TEntity, TPrimaryKey>(this IDynamicEntityPropertyManager manager, DynamicProperty dynamicProperty, int? tenantId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.AddAsync<TEntity, TPrimaryKey>(dynamicProperty.Id, tenantId);
        }
    }
}
