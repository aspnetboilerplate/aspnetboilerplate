using System;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;
using Castle.Core.Logging;

namespace Abp.EntityFrameworkCore.Repositories
{
    internal class EfCoreGenericRepositoryRegistrar : ITransientDependency
    {
        public ILogger Logger { get; set; }

        public EfCoreGenericRepositoryRegistrar()
        {
            Logger = NullLogger.Instance;
        }

        public void RegisterForDbContext(Type dbContextType, IIocManager iocManager)
        {
            var autoRepositoryAttr = dbContextType.GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ??
                                     AutoRepositoryTypesAttribute.Default;

            foreach (var entityTypeInfo in DbContextHelper.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType = autoRepositoryAttr.RepositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        var implType = autoRepositoryAttr.RepositoryImplementation.GetGenericArguments().Length == 1
                                ? autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                                : autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType);

                        iocManager.Register(
                            genericRepositoryType,
                            implType,
                            DependencyLifeStyle.Transient
                            );
                    }
                }

                var genericRepositoryTypeWithPrimaryKey = autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    var implType = autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
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