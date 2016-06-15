using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFramework.Repositories;
using Abp.MultiTenancy;

namespace Abp.EntityFramework
{
    public class DbContextTypeMatcher : IDbContextTypeMatcher, ISingletonDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly Dictionary<Type, List<Type>> _dbContextTypes;

        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _dbContextTypes = new Dictionary<Type, List<Type>>();
        }

        public void Populate(Type[] dbContextTypes)
        {
            foreach (var dbContextType in dbContextTypes)
            {
                var types = new List<Type>();
                AddWithBaseTypes(dbContextType, types);
                foreach (var type in types)
                {
                    Add(type, dbContextType);
                }
            }
        }

        public virtual Type GetConcreteType(Type sourceDbContextType)
        {
            //TODO: This can also get MultiTenancySide to filter dbcontexes

            //TODO: Can be optimized by extracting/caching MultiTenancySideAttribute attributes for DbContexes.

            //Get possible concrete types for given DbContext type
            var allTargetTypes = _dbContextTypes.GetOrDefault(sourceDbContextType);

            if (allTargetTypes.IsNullOrEmpty())
            {
                //Not found any target type, return the given type if it's not abstract
                if (sourceDbContextType.IsAbstract)
                {
                    throw new AbpException("Could not find a concrete implementation of given DbContext type: " + sourceDbContextType.AssemblyQualifiedName);
                }

                return sourceDbContextType;
            }

            if (allTargetTypes.Count == 1)
            {
                //Only one type does exists, return it
                return allTargetTypes[0];
            }

            //Will decide the target type with current UOW, so it should be in a UOW.
            if (_currentUnitOfWorkProvider.Current == null)
            {
                throw new AbpException("GetConcreteType method should be called in a UOW.");
            }

            var currentTenancySide = _currentUnitOfWorkProvider.Current.GetTenantId() == null
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;

            var multiTenancySideContexes = allTargetTypes.Where(type =>
            {
                var attrs = type.GetCustomAttributes(typeof(MultiTenancySideAttribute), true);
                if (attrs.IsNullOrEmpty())
                {
                    return false;
                }

                return ((MultiTenancySideAttribute)attrs[0]).Side.HasFlag(currentTenancySide);
            }).ToList();

            //Try to get the DbContext which is for current multitenancy side.
            if (multiTenancySideContexes.Count == 1)
            {
                return multiTenancySideContexes[0];
            }

            if (multiTenancySideContexes.Count > 1)
            {
                //Try to get the DbContext which not defined AutoRepositoryTypesAttribute
                var defaultRepositoryContexesInMultiTenancySide = multiTenancySideContexes
                    .Where(type => !type.IsDefined(typeof(AutoRepositoryTypesAttribute), true))
                    .ToList();

                if (defaultRepositoryContexesInMultiTenancySide.Count == 1)
                {
                    return defaultRepositoryContexesInMultiTenancySide[0];
                }

                throw new AbpException(string.Format(
                    "Found more than one concrete type for given DbContext Type ({0}) define MultiTenancySideAttribute with {1}. Found types: {2}.",
                    sourceDbContextType,
                    currentTenancySide,
                    multiTenancySideContexes.JoinAsString(", ")
                    ));
            }
            
            //Try to get the DbContext which not defined AutoRepositoryTypesAttribute
            var defaultRepositoryContexes = allTargetTypes
                .Where(type => !type.IsDefined(typeof(AutoRepositoryTypesAttribute), true))
                .ToList();

            if (defaultRepositoryContexes.Count == 1)
            {
                return defaultRepositoryContexes[0];
            }

            throw new AbpException(string.Format(
                "Found more than one concrete type for given DbContext Type ({0}) but none of them defines MultiTenancySideAttribute with {1}. Found types: {2}.",
                sourceDbContextType,
                currentTenancySide,
                multiTenancySideContexes.JoinAsString(", ")
                ));
        }

        private static void AddWithBaseTypes(Type dbContextType, List<Type> types)
        {
            types.Add(dbContextType);
            if (dbContextType != typeof(AbpDbContext))
            {
                AddWithBaseTypes(dbContextType.BaseType, types);
            }
        }

        private void Add(Type sourceDbContextType, Type targetDbContextType)
        {
            if (!_dbContextTypes.ContainsKey(sourceDbContextType))
            {
                _dbContextTypes[sourceDbContextType] = new List<Type>();
            }

            _dbContextTypes[sourceDbContextType].Add(targetDbContextType);
        }
    }
}