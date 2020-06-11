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

        public static void HardDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            HardDelete(repository, null, entity);
        }


        public static void HardDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            HardDelete(repository, predicate, null);
        }

        private static void HardDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var currentUnitOfWork = GetCurrentUnitOfWorkOrThrowException(repository);
            var hardDeleteEntities = currentUnitOfWork.Items.GetOrAdd(UnitOfWorkExtensionDataTypes.HardDelete, () => new HashSet<string>()) as HashSet<string>;

            var tenantId = currentUnitOfWork.GetTenantId();

            if (predicate != null)
            {
                foreach (var e in repository.GetAll().Where(predicate).ToList())
                {
                    var hardDeleteKey = EntityHelper.GetHardDeleteKey(e, tenantId);

                    hardDeleteEntities.Add(hardDeleteKey);

                    repository.Delete(e);
                }
            }

            if (entity != null)
            {
                var hardDeleteKey = EntityHelper.GetHardDeleteKey(entity, tenantId);

                hardDeleteEntities.Add(hardDeleteKey);

                repository.Delete(entity);
            }
        }

        public static async Task HardDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            await HardDeleteAsync(repository, null, entity);
        }

        public static async Task HardDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            await HardDeleteAsync(repository, predicate, null);
        }

        private static async Task HardDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var currentUnitOfWork = GetCurrentUnitOfWorkOrThrowException(repository);
            var hardDeleteEntities = currentUnitOfWork.Items.GetOrAdd(UnitOfWorkExtensionDataTypes.HardDelete, () => new HashSet<string>()) as HashSet<string>;

            var tenantId = currentUnitOfWork.GetTenantId();

            if (predicate != null)
            {
                foreach (var e in repository.GetAll().Where(predicate).ToList())
                {
                    var hardDeleteKey = EntityHelper.GetHardDeleteKey(e, tenantId);

                    hardDeleteEntities.Add(hardDeleteKey);

                    await repository.DeleteAsync(e);
                }
            }

            if (entity != null)
            {
                var hardDeleteKey = EntityHelper.GetHardDeleteKey(entity, tenantId);

                hardDeleteEntities.Add(hardDeleteKey);

                await repository.DeleteAsync(entity);
            }
        }

        private static IActiveUnitOfWork GetCurrentUnitOfWorkOrThrowException<TEntity, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>, ISoftDelete
        {
            var repo = ProxyHelper.UnProxy(repository) as IRepository<TEntity, TPrimaryKey>;
            if (repo == null)
            {
                throw new ArgumentException($"Given {nameof(repository)} is not inherited from {typeof(IRepository<TEntity, TPrimaryKey>).AssemblyQualifiedName}");
            }

            var currentUnitOfWork = ((IUnitOfWorkManagerAccessor) repo).UnitOfWorkManager.Current;
            if (currentUnitOfWork == null)
            {
                throw new AbpException($"There is no unit of work in the current context, The hard delete function can only be used in a unit of work.");
            }

            return currentUnitOfWork;
        }
    }
}
