using System;
using System.Reflection;
using Abp.MultiTenancy;
using Abp.Reflection;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// Some helper methods for entities.
    /// </summary>
    public static class EntityHelper
    {
        public static bool IsEntity(Type type)
        {
            return ReflectionHelper.IsAssignableToGenericType(type, typeof(IEntity<>));
        }

        public static Type GetPrimaryKeyType<TEntity>()
        {
            return GetPrimaryKeyType(typeof(TEntity));
        }

        /// <summary>
        /// Gets primary key type of given entity type
        /// </summary>
        public static Type GetPrimaryKeyType(Type entityType)
        {
            foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                {
                    return interfaceType.GenericTypeArguments[0];
                }
            }

            throw new AbpException("Can not find primary key type of given entity type: " + entityType + ". Be sure that this entity type implements IEntity<TPrimaryKey> interface");
        }

        public static object GetEntityId(object entity)
        {
            if (!ReflectionHelper.IsAssignableToGenericType(entity.GetType(), typeof(IEntity<>)))
            {
                throw new AbpException(entity.GetType() + " is not an Entity !");
            }

            return ReflectionHelper.GetValueByPath(entity, entity.GetType(), "Id");
        }

        public static string GetHardDeleteKey(object entity, int? tenantId)
        {
            if (MultiTenancyHelper.IsMultiTenantEntity(entity))
            {
                var tenantIdString = tenantId.HasValue ? tenantId.ToString() : "null";
                return entity.GetType().FullName + ";TenantId=" + tenantIdString + ";Id=" + GetEntityId(entity);
            }

            return entity.GetType().FullName + ";Id=" + GetEntityId(entity);
        }
    }
}