using System;
using System.Transactions;

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
        IUnitOfWorkCompleteHandle StartNew();

        IUnitOfWorkCompleteHandle StartNew(UnitOfWorkOptions options);
    }

    public class UnitOfWorkOptions
    {
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// As milliseconds.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }
        
        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }

        public UnitOfWorkOptions()
        {
            IsTransactional = true; //TODO: Remove later!
        }
    }
}
