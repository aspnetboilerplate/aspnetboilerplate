using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.Reflection;

namespace Abp.EntityFramework
{
    internal static class DbContextHelper
    {
        private static readonly ConcurrentDictionary<string, IReadOnlyList<string>> CachedTableNames = new ConcurrentDictionary<string, IReadOnlyList<string>>();

        //TODO: Get entities in different way.. we may not define DbSet for each entity.

        public static IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType)
        {
            return
                from property in dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    (ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(IDbSet<>)) ||
                     ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))) &&
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType.GenericTypeArguments[0], typeof(IEntity<>))
                select new EntityTypeInfo(property.PropertyType.GenericTypeArguments[0], property.DeclaringType);
        }

        public static IReadOnlyList<string> GetTableName(this DbContext context, Type type)
        {
            var cacheKey = context.GetType().AssemblyQualifiedName + type.AssemblyQualifiedName;
            return CachedTableNames.GetOrAdd(cacheKey, k =>
            {
                var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

                // Get the part of the model that contains info about the actual CLR types
                var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

                // Get the entity type from the model that maps to the CLR type
                var entityType = metadata
                        .GetItems<EntityType>(DataSpace.OSpace)
                        .Single(e => objectItemCollection.GetClrType(e) == type);

                // Get the entity set that uses this entity type
                var entitySet = metadata
                    .GetItems<EntityContainer>(DataSpace.CSpace)
                    .Single()
                    .EntitySets
                    .Single(s => s.ElementType.Name == entityType.Name);

                // Find the mapping between conceptual and storage model for this entity set
                var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                        .Single()
                        .EntitySetMappings
                        .Single(s => s.EntitySet == entitySet);

                // Find the storage entity sets (tables) that the entity is mapped
                var tables = mapping
                    .EntityTypeMappings.Single()
                    .Fragments;

                // Return the table name from the storage entity set
                return tables.Select(f => (string)f.StoreEntitySet.MetadataProperties["Table"].Value ?? f.StoreEntitySet.Name).ToImmutableList();
            });
        }
    }
}
