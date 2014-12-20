using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle, ITransientDependency
    {
        /// <summary>
        /// Starts the unit of work.
        /// </summary>
        /// <param name="isTransactional">Is this unit of work will be transactional</param>
        void Start(bool isTransactional = true);
    }
}