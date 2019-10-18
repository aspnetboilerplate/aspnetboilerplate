using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Reflection;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static async Task EnsureCollectionLoadedAsync<TEntity, TPrimaryKey, TProperty>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> collectionExpression,
            CancellationToken cancellationToken = default(CancellationToken)
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsExplicitLoading<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                await repo.EnsureCollectionLoadedAsync(entity, collectionExpression, cancellationToken);
            }
        }

        public static void EnsureCollectionLoaded<TEntity, TPrimaryKey, TProperty>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> collectionExpression,
            CancellationToken cancellationToken = default(CancellationToken)
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsExplicitLoading<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                repo.EnsureCollectionLoaded(entity, collectionExpression, cancellationToken);
            }
        }

        public static async Task EnsurePropertyLoadedAsync<TEntity, TPrimaryKey, TProperty>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellationToken = default(CancellationToken)
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsExplicitLoading<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                await repo.EnsurePropertyLoadedAsync(entity, propertyExpression, cancellationToken);
            }
        }

        public static void EnsurePropertyLoaded<TEntity, TPrimaryKey, TProperty>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellationToken = default(CancellationToken)
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsExplicitLoading<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                repo.EnsurePropertyLoaded(entity, propertyExpression, cancellationToken);
            }
        }

        public static IIocResolver GetIocResolver<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var repo = ProxyHelper.UnProxy(repository) as AbpRepositoryBase<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                return repo.IocResolver;
            }

            throw new ArgumentException($"Given {nameof(repository)} is not inherited from {typeof(AbpRepositoryBase<TEntity, TPrimaryKey>).AssemblyQualifiedName}");
        }

        public static async Task HardDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var repo = ProxyHelper.UnProxy(repository) as IRepository<TEntity, TPrimaryKey>;
            if (repo == null)
            {
                throw new ArgumentException($"Given {nameof(repository)} is not inherited from {typeof(IRepository<TEntity, TPrimaryKey>).AssemblyQualifiedName}");
            }

            var items = ((IUnitOfWorkManagerAccessor)repo).UnitOfWorkManager.Current.Items;
            var hardDeleteEntities = items.GetOrAdd(UnitOfWorkExtensionDataTypes.HardDelete, () => new HashSet<string>()) as HashSet<string>;

            var tenantId = GetCurrentTenantIdOrNull(repo.GetIocResolver());
            var hardDeleteKey = EntityHelper.GetHardDeleteKey(entity, tenantId);

            hardDeleteEntities.Add(hardDeleteKey);

            await repo.DeleteAsync(entity);
        }

        public static void HardDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var repo = ProxyHelper.UnProxy(repository) as IRepository<TEntity, TPrimaryKey>;
            if (repo == null)
            {
                throw new ArgumentException($"Given {nameof(repository)} is not inherited from {typeof(IRepository<TEntity, TPrimaryKey>).AssemblyQualifiedName}");
            }

            var items = ((IUnitOfWorkManagerAccessor)repo).UnitOfWorkManager.Current.Items;
            var hardDeleteEntities = items.GetOrAdd(UnitOfWorkExtensionDataTypes.HardDelete, () => new HashSet<string>()) as HashSet<string>;

            var tenantId = GetCurrentTenantIdOrNull(repo.GetIocResolver());
            var hardDeleteKey = EntityHelper.GetHardDeleteKey(entity, tenantId);

            hardDeleteEntities.Add(hardDeleteKey);

            repo.Delete(entity);
        }

        public static async Task HardDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            foreach (var entity in repository.GetAll().Where(predicate).ToList())
            {
                await repository.HardDeleteAsync(entity);
            }
        }

        public static void HardDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            foreach (var entity in repository.GetAll().Where(predicate).ToList())
            {
                repository.HardDelete(entity);
            }
        }

        private static int? GetCurrentTenantIdOrNull(IIocResolver iocResolver)
        {
            using (var scope = iocResolver.CreateScope())
            {
                var currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();
                if (currentUnitOfWorkProvider?.Current != null)
                {
                    return currentUnitOfWorkProvider.Current.GetTenantId();
                }
            }

            return iocResolver.Resolve<IAbpSession>().TenantId;
        }
    }
}
