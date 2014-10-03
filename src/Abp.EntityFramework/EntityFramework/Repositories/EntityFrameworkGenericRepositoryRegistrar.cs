using System;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    internal static class EntityFrameworkGenericRepositoryRegistrar
    {
        public static void RegisterDbContext(Type dbContextType, IIocManager iocManager)
        {
            foreach (var entityType in EntityFrameworkHelper.GetEntityTypesInDbContext(dbContextType))
            {
                foreach (var interfaceType in entityType.GetInterfaces())
                {
                    if (interfaceType == typeof(IEntity))
                    {
                        iocManager.Register(
                            typeof(IRepository<>).MakeGenericType(entityType),
                            typeof(EfRepositoryBase<,>).MakeGenericType(dbContextType, entityType),
                            DependencyLifeStyle.Transient
                            );
                    }
                    else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                    {
                        var primaryKeyType = interfaceType.GenericTypeArguments[0];
                        iocManager.Register(
                            typeof(IRepository<,>).MakeGenericType(entityType, primaryKeyType),
                            typeof(EfRepositoryBase<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType),
                            DependencyLifeStyle.Transient
                            );
                    }
                }
            }
        }
    }
}