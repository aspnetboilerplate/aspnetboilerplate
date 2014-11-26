using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// </summary>
    public interface IUnitOfWork : ITransientDependency, IDisposable
    {
        /// <summary>
        /// Gets if this unit of work is transactional
        /// </summary>
        bool IsTransactional { get; }

        /// <summary>
        /// Initializes this unit of work.
        /// </summary>
        /// <param name="isTransactional">Is this unit of work will be transactional?</param>
        void Initialize(bool isTransactional);

        /// <summary>
        /// Starts this unit of woek.
        /// </summary>
        void Begin();

        /// <summary>
        /// Saves all changes until now in this unit of work.
        /// This method may be called to apply changes whenever needed.
        /// Note that if this unit of work is transactional, saved changes are also rolled back
        /// if transaction is rolled back.
        /// No explicit call is needed to SaveChanges generally, 
        /// since all changes saved at end of a unit of work automatically.
        /// </summary>
        void SaveChanges();

        Task SaveChangesAsync();

        /// <summary>
        /// Ends this unit of work.
        /// It saves all changes and commit transaction if exists.
        /// </summary>
        void End();

        /// <summary>
        /// Cancels current unit of work.
        /// Rollbacks transaction if exists.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Add a handler that will be called if unit of work succeed.
        /// </summary>
        /// <param name="action">Action to be executed</param>
        void OnSuccess(Action action);
    }
}