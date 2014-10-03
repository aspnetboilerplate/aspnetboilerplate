using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Abp.Reflection;

namespace Abp.EntityFramework
{
    internal static class EntityFrameworkHelper
    {
        public static IEnumerable<Type> GetEntityTypesInDbContext(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(IDbSet<>)) ||
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))
                select property.PropertyType.GenericTypeArguments[0];
        }
    }
}