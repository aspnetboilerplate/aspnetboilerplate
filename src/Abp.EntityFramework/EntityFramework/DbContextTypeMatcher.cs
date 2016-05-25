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
        private readonly Dictionary<Type, List<Type>> _dbContextTypes;

        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _dbContextTypes = new Dictionary<Type, List<Type>>();
        }

        public virtual void Add(Type sourceDbContextType, Type targetDbContextType)
        {
            if (!_dbContextTypes.ContainsKey(sourceDbContextType))
            {
                _dbContextTypes[sourceDbContextType] = new List<Type>();
            }

            _dbContextTypes[sourceDbContextType].Add(targetDbContextType);
        }

        public virtual Type GetConcreteType(Type sourceDbContextType)
        {
            //TODO: Can be optimized by extracting/caching MultiTenancySideAttribute attributes for DbContexes.

            //Get possible concrete types for given DbContext type
            var targetList = _dbContextTypes.GetOrDefault(sourceDbContextType);

            if (targetList.IsNullOrEmpty())
            {
                //Not found any target type, return the given type if it's not abstract
                if (sourceDbContextType.IsAbstract)
                {
                    throw new AbpException("Could not find a concrete implementation of given DbContext type: " + sourceDbContextType.AssemblyQualifiedName);
                }

                return sourceDbContextType;
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
                if (attrs.IsNullOrEmpty())
                {
                    return false;
                }

                return ((MultiTenancySideAttribute) attrs[0]).Side.HasFlag(currentTenancySide);
            }).ToList();

            if (targetList.Count < 1)
            {
                throw new AbpException(string.Format(
                    "Found more than one concrete type for given DbContext Type ({0}) but none of them defines MultiTenancySideAttribute with {1}",
                    sourceDbContextType,
                    currentTenancySide
                    ));
            }
            if (targetList.Count > 1)
            {
                throw new AbpException(string.Format(
                    "Found more than one concrete type for given DbContext Type ({0}) define MultiTenancySideAttribute with {1}",
                    sourceDbContextType,
                    currentTenancySide
                    ));
            }

            return targetList[0];
        }
    }
}