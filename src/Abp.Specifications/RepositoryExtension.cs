namespace Abp
{
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;

    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Abp.Specifications;

    /// <summary>
    /// The repository extension.
    /// </summary>
    public static class RepositoryExtension
    {
        /// <summary>
        /// The is exist async.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<bool> IsExistAsync<T, TKey>(
            this IRepository<T, TKey> rep,
            ISpecification<T> specification) where T : Entity<TKey>
        {
            CheckSpecification(specification);
            var query = rep.GetAll();
            return await Task.FromResult(query.Where(specification.GetExpression()).Any());
        }

        /// <summary>
        /// The find.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async static Task<T> Find<T,TKey>
            (
            this IRepository<T, TKey> rep,
            ISpecification<T> specification) where T : Entity<TKey>
        {
            CheckSpecification(specification);
            return await rep.FirstOrDefaultAsync(specification.GetExpression());
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<IQueryable<T>> FindAll<T, TKey>(
this IRepository<T, TKey> rep,
ISpecification<T> specification)
where T : Entity<TKey>
        {
            CheckSpecification(specification);
            return await Task.FromResult(rep.GetAll().Where(specification.GetExpression()));
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="orderBy">
        /// The order by.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<IQueryable<T>> FindAll<T, TKey>(
    this IRepository<T, TKey> rep,
    string orderBy)
    where T : Entity<TKey>
        {
            var query = rep.GetAll();
            return await Task.FromResult(query
                .OrderBy(orderBy));
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <param name="orderBy">
        /// The order by.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<IQueryable<T>> FindAll<T, TKey>(
        this IRepository<T, TKey> rep,
        ISpecification<T> specification,
        string orderBy)
        where T : Entity<TKey>
        {
            CheckSpecification(specification);
            var query = rep.GetAll();
            
            return await Task.FromResult(query
                .OrderBy(orderBy));
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <param name="skip">
        /// The skip.
        /// </param>
        /// <param name="take">
        /// The take.
        /// </param>
        /// <param name="orderBy">
        /// The order by.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<IQueryable<T>> FindAll<T, TKey>(
            this IRepository<T, TKey> rep,
            ISpecification<T> specification,
            int skip,
            int take, 
            string orderBy)
            where T : Entity<TKey>
        {
            CheckSpecification(specification);
            var query = rep.GetAll();
            
            return await Task.FromResult(query
                .OrderBy(orderBy)
                .Skip(skip)
                .Take(take));
        }

        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<int> Count<T, TKey>(
            this IRepository<T, TKey> rep, 
            ISpecification<T> specification)
            where T : Entity<TKey>
        {
            CheckSpecification(specification);
            return await rep.CountAsync(specification.GetExpression());
        }

        /// <summary>
        /// The checkspecification.
        /// </summary>
        /// <param name="specification">
        /// The specification.
        /// </param>
        /// <typeparam name="T">
        /// Entity
        /// </typeparam>
        /// <exception cref="AbpException">
        /// </exception>
        private static void CheckSpecification<T>(ISpecification<T> specification)
        {
            if (specification == null)
            {
                throw new AbpException("specification is null");
            }
        }
    }
}