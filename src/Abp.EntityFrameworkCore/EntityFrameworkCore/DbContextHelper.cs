using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore
{
    internal static class DbContextHelper
    {
        //TODO: Get entities in different way.. we may not define DbSet for each entity.

        public static IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType.GenericTypeArguments[0], typeof(IEntity<>))
                select new EntityTypeInfo(property.PropertyType.GenericTypeArguments[0], property.DeclaringType);
        }
    }
}