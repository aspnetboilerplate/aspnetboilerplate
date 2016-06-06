using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.GraphDiff.Mapping;
using RefactorThis.GraphDiff;

namespace Abp.GraphDiff.Extensions
{
    /// <summary>
    /// This class is an extension for GraphDiff library which provides a possibility to attach a detached graphs (i.e. entities) to a context.
    /// Attaching a whole graph using this methods updates all entity's navigation properties on entity creation or modification.
    /// </summary>
    public static class GraphDiffExtensions
    {
        /// <summary>
        /// Attaches an <paramref name="entity"/> (as a detached graph) to a context.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns>Attached entity</returns>
        public static TEntity AttachGraph<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;

            var mappingManager = iocResolver.Resolve<IEntityMappingManager>();
            var mapping = mappingManager.GetEntityMappingOrNull<TEntity>();

            var context = repository.GetDbContext();
            var insertedEntity = context.UpdateGraph(entity, mapping);

            return insertedEntity;
        }

        /// <summary>
        /// Attaches an <paramref name="entity"/> (as a detached graph) to a context.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns>Attached entity</returns>
        public static Task<TEntity> AttachGraphAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            return Task.FromResult(AttachGraph(repository, entity));
        }

        /// <summary>
        /// Attaches an <paramref name="entity"/> (as a detached graph) to a context and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns>Id of the entity</returns>
        public static TPrimaryKey AttachGraphAndGetId<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            return AttachGraph(repository, entity).Id;
        }

        /// <summary>
        /// Attaches an <paramref name="entity"/> (as a detached graph) to a context and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns>Id of the entity</returns>
        public static Task<TPrimaryKey> AttachGraphAndGetIdAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            return Task.FromResult(AttachGraphAndGetId(repository, entity));
        }
    }
}