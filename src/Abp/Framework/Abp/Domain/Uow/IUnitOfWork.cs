using System;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// </summary>
    public interface IUnitOfWork : ITransientDependency, IDisposable
    {
        /// <summary>
        /// Begins to the unit of work.
        /// </summary>
        /// <param name="isTransactional"></param>
        void Begin(bool isTransactional);

        /// <summary>
        /// Ends tis unit of work.
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
        void AddSuccessHandler(Action action);
    }
}