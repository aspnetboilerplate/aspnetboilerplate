namespace Abp.Domain.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Abp.Domain.Specifications;

    using Castle.Core.Internal;

    /// <summary>
    /// The repository extensions.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    /// <typeparam name="TPrimaryKey">
    /// </typeparam>
    public static class RepositoryExtensions
    {
        #region Select/Get

        public static IQueryable<TEntity> GetAll<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {
            return respoitory.GetAll().Where(specification.GetExpression());
        }

        public static bool IsExist<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {

            return respoitory.GetAll().Any(specification.GetExpression());
        }

        public static Task<bool> IsExistAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.FromResult(IsExist(respoitory, specification));
        }

        public static List<TEntity> GetAllList<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {
            return respoitory.GetAllList(specification.GetExpression());
        }

        public static Task<List<TEntity>> GetAllListAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.FromResult(GetAllList(respoitory, specification));
        }

        public static TEntity Single<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) where TEntity : class, IEntity<TPrimaryKey>
        {
            return respoitory.Single(specification.GetExpression());
        }

        public static Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.FromResult(Single(respoitory, specification));
        }

        #endregion

        #region Detele

        public static void Delete<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            respoitory.Delete(specification.GetExpression());
        }

        public static Task DeleteAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return respoitory.DeleteAsync(specification.GetExpression());
        }

        #endregion

        public static int Count<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return respoitory.Count(specification.GetExpression());
        }

        public static Task<int> CountAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> respoitory,
            ISpecification<TEntity> specification)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.FromResult(Count(respoitory, specification));
        }




    }
}