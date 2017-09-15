using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Orm;
using Abp.Reflection.Extensions;

namespace Abp.EntityFramework
{
    public abstract class SecondaryOrmRegistrarBase : ISecondaryOrmRegistrar
    {
        private readonly IDbContextEntityFinder _dbContextEntityFinder;
        private readonly Type _dbContextType;

        protected SecondaryOrmRegistrarBase(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
        {
            _dbContextType = dbContextType;
            _dbContextEntityFinder = dbContextEntityFinder;
        }

        public abstract string OrmContextKey { get; }

        public virtual void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes)
        {
            AutoRepositoryTypesAttribute autoRepositoryAttr = _dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>()
                                                              ?? defaultRepositoryTypes;

            foreach (EntityTypeInfo entityTypeInfo in _dbContextEntityFinder.GetEntityTypeInfos(_dbContextType))
            {
                Type primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    Type genericRepositoryType = autoRepositoryAttr.RepositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        Type implType = autoRepositoryAttr.RepositoryImplementation.GetTypeInfo().GetGenericArguments().Length == 1
                            ? autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType);

                        iocManager.Register(
                            genericRepositoryType,
                            implType,
                            DependencyLifeStyle.Transient
                        );
                    }
                }

                Type genericRepositoryTypeWithPrimaryKey = autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    Type implType = autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.GetTypeInfo().GetGenericArguments().Length == 2
                        ? autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                        : autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType, primaryKeyType);

                    iocManager.Register(
                        genericRepositoryTypeWithPrimaryKey,
                        implType,
                        DependencyLifeStyle.Transient
                    );
                }
            }
        }
    }
}
