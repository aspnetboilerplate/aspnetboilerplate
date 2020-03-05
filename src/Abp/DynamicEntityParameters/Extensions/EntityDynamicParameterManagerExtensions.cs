using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters.Extensions
{
    public static class EntityDynamicParameterManagerExtensions
    {
        public static List<EntityDynamicParameter> GetAll<TEntity, TPrimaryKey>(this EntityDynamicParameterManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAll(entityFullName: typeof(TEntity).FullName);
        }

        public static List<EntityDynamicParameter> GetAll<TEntity>(this EntityDynamicParameterManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAll<TEntity, int>();
        }

        public static Task<List<EntityDynamicParameter>> GetAllAsync<TEntity, TPrimaryKey>(this EntityDynamicParameterManager manager)
            where TEntity : IEntity<TPrimaryKey>
        {
            return manager.GetAllAsync(entityFullName: typeof(TEntity).FullName);
        }

        public static Task<List<EntityDynamicParameter>> GetAllAsync<TEntity>(this EntityDynamicParameterManager manager)
            where TEntity : IEntity<int>
        {
            return manager.GetAllAsync<TEntity, int>();
        }
    }
}
