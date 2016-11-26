namespace Abp
{
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;

    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
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
            AbpGuard.NotNull(specification, "specification");

            var query = rep.GetAll();
            return await Task.Run(() => query.WhereIf(specification != null, specification.GetExpression()).Any());
        }
        
        public static async Task<IQueryable<T>> FindAll<T, TKey>(
this IRepository<T, TKey> rep,
ISpecification<T> specification)
where T : Entity<TKey>
        {
            AbpGuard.NotNull(specification, "specification");
            var query = rep.GetAll();

            var result = query.Where(specification.GetExpression());

            return await Task.Run(() => result);
        }

        public static async Task<IQueryable<T>> FindAll<T, TKey>(
    this IRepository<T, TKey> rep,
    string orderBy)
    where T : Entity<TKey>
        {
            var query = rep.GetAll();
            
            var result = query
                .OrderBy(orderBy);

            return await Task.Run(() => result);
        }

        public static async Task<IQueryable<T>> FindAll<T, TKey>(
        this IRepository<T, TKey> rep,
        ISpecification<T> specification,
        string orderBy)
        where T : Entity<TKey>
        {
            AbpGuard.NotNull(specification, "specification");
            var query = rep.GetAll();
            query = query.WhereIf(specification != null, specification.GetExpression());

            var result = query
                .OrderBy(orderBy);

            return await Task.Run(() => result);
        }

        public static async Task<IQueryable<T>> FindAll<T, TKey>(
            this IRepository<T, TKey> rep,
            ISpecification<T> specification,
            int skip,
            int take, 
            string orderBy)
            where T : Entity<TKey>
        {
            AbpGuard.NotNull(specification, "specification");
            var query = rep.GetAll();
            query = query.WhereIf(specification != null, specification.GetExpression());

            var result = query
                .OrderBy(orderBy)
                .Skip(skip)
                .Take(take);

            return await Task.Run(() =>result);
        }
        
        public static async Task<int> Count<T, TKey>(
            this IRepository<T, TKey> rep, 
            ISpecification<T> specification)
            where T : Entity<TKey>
        {
            AbpGuard.NotNull(specification, "specification");
            var query = rep.GetAll();
            return await Task.Run(() =>
            query.WhereIf(specification != null,
            specification.GetExpression()).Count());
        }



    }
}