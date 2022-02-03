using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;

namespace Abp.EntityFramework.Repositories
{
    public class EfGenericRepositoryRegistrar : IEfGenericRepositoryRegistrar, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IDbContextEntityFinder _dbContextEntityFinder;

        public EfGenericRepositoryRegistrar(IDbContextEntityFinder dbContextEntityFinder)
        {
            _dbContextEntityFinder = dbContextEntityFinder;
            Logger = NullLogger.Instance;
        }

        public void RegisterForDbContext(
            Type dbContextType, 
            IIocManager iocManager, 
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute)
        {
            var autoRepositoryAttr = dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ?? defaultAutoRepositoryTypesAttribute;

            RegisterForDbContext(
                dbContextType,
                iocManager,
                autoRepositoryAttr.RepositoryInterface,
                autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey,
                autoRepositoryAttr.RepositoryImplementation,
                autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
            );

            if (autoRepositoryAttr.WithDefaultRepositoryInterfaces)
            {
                RegisterForDbContext(
                    dbContextType,
                    iocManager,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterface,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterfaceWithPrimaryKey,
                    autoRepositoryAttr.RepositoryImplementation,
                    autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
                );
            }
        }

        public void RegisterForEntity(
            Type dbContextType, 
            Type entityType, 
            IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute)
        {
            var autoRepositoryAttr =
                dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ??
                defaultAutoRepositoryTypesAttribute;
            
            RegisterForEntity(
                dbContextType,
                entityType,
                iocManager,
                defaultAutoRepositoryTypesAttribute.RepositoryInterface,
                defaultAutoRepositoryTypesAttribute.RepositoryInterfaceWithPrimaryKey,
                autoRepositoryAttr.RepositoryImplementation,
                autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
            );
        }
        
        private static void RegisterForEntity(
            Type dbContextType,
            Type entityType,
            IIocManager iocManager, 
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey, 
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityType);
            if (primaryKeyType == typeof(int))
            {
                var genericRepositoryType = repositoryInterface.MakeGenericType(entityType);
                if (!iocManager.IsRegistered(genericRepositoryType))
                {
                    var implType = repositoryImplementation.GetGenericArguments().Length == 1
                        ? repositoryImplementation.MakeGenericType(entityType)
                        : repositoryImplementation.MakeGenericType(dbContextType, entityType);

                    iocManager.IocContainer.Register(
                        Component
                            .For(genericRepositoryType)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }

            var genericRepositoryTypeWithPrimaryKey = repositoryInterfaceWithPrimaryKey.MakeGenericType(
                entityType,
                primaryKeyType
            );

            if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
            {
                var implType = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                    ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityType, primaryKeyType)
                    : repositoryImplementationWithPrimaryKey.MakeGenericType(dbContextType,
                        entityType, primaryKeyType);

                iocManager.IocContainer.Register(
                    Component
                        .For(genericRepositoryTypeWithPrimaryKey)
                        .ImplementedBy(implType)
                        .Named(Guid.NewGuid().ToString("N"))
                        .LifestyleTransient()
                );
            }
        }

        private void RegisterForDbContext(
            Type dbContextType, 
            IIocManager iocManager,
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            foreach (var entityTypeInfo in _dbContextEntityFinder.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType = repositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        var implType = repositoryImplementation.GetGenericArguments().Length == 1
                            ? repositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : repositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                                entityTypeInfo.EntityType);

                        iocManager.IocContainer.Register(
                            Component
                                .For(genericRepositoryType)
                                .ImplementedBy(implType)
                                .Named(Guid.NewGuid().ToString("N"))
                                .LifestyleTransient()
                        );
                    }
                }

                var genericRepositoryTypeWithPrimaryKey = repositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType,primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    var implType = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                        ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                        : repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType, primaryKeyType);

                    iocManager.IocContainer.Register(
                        Component
                            .For(genericRepositoryTypeWithPrimaryKey)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }
        }
    }
}
