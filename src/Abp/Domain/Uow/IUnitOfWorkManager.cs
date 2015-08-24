using System.Transactions;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// Used to begin and control a unit of work.
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// Gets currently active unit of work (or null if not exists).
        /// </summary>
        IActiveUnitOfWork Current { get; }

        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle Begin();

        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope);

        /// <summary>
        /// Begins a new unit of work.
        /// </summary>
        /// <returns>A handle to be able to complete the unit of work</returns>
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}
