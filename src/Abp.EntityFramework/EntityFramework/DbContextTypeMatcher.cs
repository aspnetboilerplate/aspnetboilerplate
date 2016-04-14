using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;

namespace Abp.EntityFramework
{
    public class DbContextTypeMatcher : IDbContextTypeMatcher, ISingletonDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        public Dictionary<Type, List<Type>> _types;

        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _types = new Dictionary<Type, List<Type>>();
        }

        public void Add(Type sourceType, Type targetType)
        {
            if (!_types.ContainsKey(sourceType))
            {
                _types[sourceType] = new List<Type>();
            }

            _types[sourceType].Add(targetType);
        }

        public Type GetConcreteType(Type dbContextType)
        {
            //TODO: Can be optimized by extracting/caching MultiTenancySideAttribute attributes for DbContexes.

            //Get possible concrete types for given DbContext type
            var targetList = _types.GetOrDefault(dbContextType);

            if (targetList.IsNullOrEmpty())
            {
                //Not found any target type, return the given type if it's not abstract
                if (dbContextType.IsAbstract)
                {
                    throw new AbpException("Could not find a concrete implementation of given DbContext type: " + dbContextType.AssemblyQualifiedName);
                }

                return dbContextType;
            }

            if (targetList.Count == 1)
            {
                //Only one type does exists, return it
                return targetList[0];
            }

            //Will decide the target type with current UOW, so it should be in a UOW.
            if (_currentUnitOfWorkProvider.Current == null)
            {
                throw new AbpException("GetConcreteType method should be called in a UOW.");
            }

            var currentTenancySide = _currentUnitOfWorkProvider.Current.GetTenantId() == null
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;

            targetList = targetList.Where(type =>
            {
                var attrs = type.GetCustomAttributes(typeof (MultiTenancySideAttribute), true);
                if (CollectionExtensions.IsNullOrEmpty<object>(attrs))
                {
                    return false;
                }

                return ((MultiTenancySideAttribute) attrs[0]).Side.HasFlag(currentTenancySide);
            }).ToList();

            if (targetList.Count < 1)
            {
                throw new AbpException(string.Format(
                    "Found more than one concrete type for given DbContext Type ({0}) but none of them defines MultiTenancySideAttribute with {1}",
                    dbContextType,
                    currentTenancySide
                    ));
            }
            if (targetList.Count > 1)
            {
                throw new AbpException(string.Format(
                    "Found more than one concrete type for given DbContext Type ({0}) define MultiTenancySideAttribute with {1}",
                    dbContextType,
                    currentTenancySide
                    ));
            }

            return targetList[0];
        }
    }
}