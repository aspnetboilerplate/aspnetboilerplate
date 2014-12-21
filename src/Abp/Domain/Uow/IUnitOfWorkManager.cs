namespace Abp.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// Gets currently active unit of work (or null if not exists).
        /// </summary>
        IActiveUnitOfWork Current { get; }

        /// <summary>
        /// Starts a new unit of work.
        /// </summary>
        /// <param name="isTransactional">Is this unit of work will be transactional</param>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle StartNew(bool isTransactional = true);
    }
}
