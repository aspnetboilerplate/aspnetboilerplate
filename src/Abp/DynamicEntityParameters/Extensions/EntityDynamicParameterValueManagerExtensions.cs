using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters.Extensions
{
    public static class EntityDynamicParameterValueManagerExtensions
    {
        public static List<EntityDynamicParameterValue> GetValues<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityRowId: entityRowId);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityRowId: entityRowId);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, int dynamicParameterId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId, dynamicParameterId: dynamicParameterId);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, int dynamicParameterId)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityRowId: entityRowId, dynamicParameterId: dynamicParameterId);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, int dynamicParameterId)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId, dynamicParameterId: dynamicParameterId);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, int dynamicParameterId)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityRowId: entityRowId, dynamicParameterId: dynamicParameterId);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, DynamicParameter dynamicParameter)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues<TEntity, TPrimaryKey>(entityRowId: entityRowId, dynamicParameterId: dynamicParameter.Id);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, DynamicParameter dynamicParameter)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity>(entityRowId: entityRowId, dynamicParameterId: dynamicParameter.Id);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, DynamicParameter dynamicParameter)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync<TEntity, TPrimaryKey>(entityRowId: entityRowId, dynamicParameterId: dynamicParameter.Id);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, DynamicParameter dynamicParameter)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity>(entityRowId: entityRowId, dynamicParameterId: dynamicParameter.Id);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, string parameterName)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValues(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId, parameterName: parameterName);
        }

        public static List<EntityDynamicParameterValue> GetValues<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, string parameterName)
            where TEntity : IEntity<int>
        {
            return manager.GetValues<TEntity, int>(entityRowId: entityRowId, parameterName: parameterName);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity, TPrimaryKey>(this IEntityDynamicParameterValueManager manager, string entityRowId, string parameterName)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetValuesAsync(entityFullName: typeof(TEntity).FullName, entityRowId: entityRowId, parameterName: parameterName);
        }

        public static Task<List<EntityDynamicParameterValue>> GetValuesAsync<TEntity>(this IEntityDynamicParameterValueManager manager, string entityRowId, string parameterName)
            where TEntity : IEntity<int>
        {
            return manager.GetValuesAsync<TEntity, int>(entityRowId: entityRowId, parameterName: parameterName);
        }
    }
}
