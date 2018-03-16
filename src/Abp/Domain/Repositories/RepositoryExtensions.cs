using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Localization;
using Abp.Reflection;
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
            Expression<Func<TEntity, IEnumerable<TProperty>>> collectionExpression
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            AsyncHelper.RunSync(() => repository.EnsureCollectionLoadedAsync(entity, collectionExpression));
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
            Expression<Func<TEntity, TProperty>> propertyExpression
        )
            where TEntity : class, IEntity<TPrimaryKey>
            where TProperty : class
        {
            AsyncHelper.RunSync(() => repository.EnsurePropertyLoadedAsync(entity, propertyExpression));
        }

        public static IIocResolver GetIocResolver<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var repo = ProxyHelper.UnProxy(repository) as AbpRepositoryBase<TEntity, TPrimaryKey>;
            if (repo != null)
            {
                return repo.IocManager;
            }

            throw new ArgumentException($"Given {nameof(repository)} is not inherited from {typeof(AbpRepositoryBase<TEntity, TPrimaryKey>).AssemblyQualifiedName}");
        }

        public static async Task<TTranslation> GetWithFallback<TTranslation, TCore>(
            this IRepository<TTranslation> repository, int id
            )
            where TTranslation : class, IEntityTranslation<TCore>, IEntity
            where TCore : class, IMultiLingualEntity<TTranslation>
        {
            if (!(ProxyHelper.UnProxy(repository) is AbpRepositoryBase<TTranslation, int> repo))
            {
                throw new Exception("The repository is not derived from AbpRepositoryBase.");
            }

            //todo@ismail: Should we do this because now we have query filters ???
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            var translationInCurrentLanguage = repo.FirstOrDefault(e => e.CoreId == id && e.Language == currentCulture);
            if (translationInCurrentLanguage != null)
            {
                return translationInCurrentLanguage;
            }

            var defaultCulture = await repo.IocManager.Resolve<ISettingManager>().GetSettingValueAsync(LocalizationSettingNames.DefaultLanguage);
            if (string.IsNullOrEmpty(defaultCulture))
            {
                return null;
            }

            return repo.FirstOrDefault(e => e.CoreId == id && e.Language == defaultCulture);
        }

        public static async Task<TCore> GetWithTranslation<TCore, TTranslation>(
            this IRepository<TCore> repository, int id
        )
            where TTranslation : class, IEntityTranslation<TCore>, IEntity
            where TCore : class, IMultiLingualEntity<TTranslation>
        {
            if (!(ProxyHelper.UnProxy(repository) is AbpRepositoryBase<TCore, int> repo))
            {
                throw new Exception("The repository is not derived from AbpRepositoryBase.");
            }

            return repo.GetAllIncluding(core => core.Translations)
                .FirstOrDefault(core => core.Id == id);
        }

        public static async Task<IQueryable<TCore>> GetAllIncludingTranslation<TCore, TTranslation>(
            this IRepository<TCore> repository,
            Expression<Func<TCore, bool>> filterExpression = null
        )
            where TTranslation : class, IEntityTranslation<TCore>, IEntity
            where TCore : class, IMultiLingualEntity<TTranslation>
        {
            if (!(ProxyHelper.UnProxy(repository) is AbpRepositoryBase<TCore, int> repo))
            {
                throw new Exception("The repository is not derived from AbpRepositoryBase.");
            }

            if (filterExpression == null)
            {
                return repo.GetAllIncluding(core => core.Translations);
            }

            return repo.GetAllIncluding(core => core.Translations)
                .Where(filterExpression);
        }
    }
}
