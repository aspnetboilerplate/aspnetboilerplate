using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityProperties.Extensions
{
    public static class DynamicEntityPropertyValueManagerExtensions
    {
        public static List<DynamicEntityPropertyValue> GetValues<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(entityFullName: typeof(TEntity).FullName, entityId: entityId);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityId);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(entityFullName: typeof(TEntity).FullName, entityId: entityId);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityId);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, int dynamicPropertyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(typeof(TEntity).FullName, entityId, dynamicPropertyId);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, int dynamicPropertyId)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityId, dynamicPropertyId);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, int dynamicPropertyId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(typeof(TEntity).FullName, entityId, dynamicPropertyId);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, int dynamicPropertyId)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityId, dynamicPropertyId);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, DynamicProperty dynamicProperty)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues<TEntity, TPrimaryKey>(entityId, dynamicProperty.Id);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, DynamicProperty dynamicProperty)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity>(entityId, dynamicProperty.Id);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, DynamicProperty dynamicProperty)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync<TEntity, TPrimaryKey>(entityId, dynamicProperty.Id);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, DynamicProperty dynamicProperty)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity>(entityId, dynamicProperty.Id);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, string propertyName)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(typeof(TEntity).FullName, entityId, propertyName);
        }

        public static List<DynamicEntityPropertyValue> GetValues<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, string propertyName)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityId, propertyName);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity, TPrimaryKey>(
            this IDynamicEntityPropertyValueManager manager, string entityId, string propertyName)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(typeof(TEntity).FullName, entityId, propertyName);
        }

        public static Task<List<DynamicEntityPropertyValue>> GetValuesAsync<TEntity>(
            this IDynamicEntityPropertyValueManager manager, string entityId, string propertyName)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityId, propertyName);
        }
    }
}