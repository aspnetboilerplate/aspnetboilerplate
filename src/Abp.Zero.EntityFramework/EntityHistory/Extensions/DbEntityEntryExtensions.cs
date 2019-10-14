using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace Abp.EntityHistory.Extensions
{
    internal static class DbEntityEntryExtensions
    {
        internal static Type GetEntityBaseType(this DbEntityEntry entityEntry)
        {
            return ObjectContext.GetObjectType(entityEntry.Entity.GetType());
        }

        internal static PropertyInfo GetPropertyInfo(this DbEntityEntry entityEntry, string propertyName)
        {
            return entityEntry.GetEntityBaseType().GetProperty(propertyName);
        }

        internal static bool IsCreated(this DbEntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        internal static bool IsDeleted(this DbEntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }
            var entity = entityEntry.Entity;
            return entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted;
        }
    }
}