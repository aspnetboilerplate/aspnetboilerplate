using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Specifications;

namespace Abp.Domain.Repositories
{
    //TODO: Sync versions of the methods!
    //TODO: Other methods of the IRepository takes expression and returns list/entity?

    /// <summary>
    /// The repository extension.
    /// </summary>
    public static class RepositorySpecificationExtensions
    {
        public static Task<T> FirstOrDefaultAsync<T, TPrimaryKey>(this IRepository<T, TPrimaryKey> repository, ISpecification<T> specification)
            where T : Entity<TPrimaryKey>
        {
            Check.NotNull(specification, nameof(specification));

            return repository.FirstOrDefaultAsync(specification.ToExpression());
        }

        public static Task<int> Count<T, TPrimaryKey>(this IRepository<T, TPrimaryKey> repository, ISpecification<T> specification)
            where T : Entity<TPrimaryKey>
        {
            Check.NotNull(specification, nameof(specification));

            return repository.CountAsync(specification.ToExpression());
        }
    }
}