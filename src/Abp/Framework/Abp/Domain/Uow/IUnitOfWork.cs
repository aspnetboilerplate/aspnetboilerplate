using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// </summary>
    public interface IUnitOfWork : ITransientDependency
    {
        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <param name="isTransactional"></param>
        void Begin(bool isTransactional);

        /// <summary>
        /// Commits the unit of work.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks the unit of work.
        /// </summary>
        void Rollback();
    }
}