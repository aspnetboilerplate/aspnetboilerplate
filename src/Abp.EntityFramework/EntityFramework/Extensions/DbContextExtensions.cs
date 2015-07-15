using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Abp.Reflection;

namespace Abp.EntityFramework.Extensions
{
    internal static class DbContextExtensions
    {
        public static IEnumerable<Type> GetEntityTypes(this Type dbContextType, bool includeBase = true)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    (ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(IDbSet<>)) ||
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))) &&
                    (includeBase || property.DeclaringType == dbContextType)
                select property.PropertyType.GenericTypeArguments[0];
        }
    }
}