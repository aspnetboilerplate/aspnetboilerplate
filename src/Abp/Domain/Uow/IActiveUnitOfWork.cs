using System;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interface is used to work with active unit of work.
    /// This interface can not be injected.
    /// Use <see cref="IUnitOfWorkManager"/> instead.
    /// </summary>
    public interface IActiveUnitOfWork
    {
        /// <summary>
        /// This event is raised when this UOW is successfully completed.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// This event is raised when this UOW is failed.
        /// </summary>
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <summary>
        /// This event is raised when this UOW is disposed.
        /// </summary>
        event EventHandler Disposed;
        
        /// <summary>
        /// Gets if this unit of work is transactional.
        /// </summary>
        UnitOfWorkOptions Options { get; }

        /// <summary>
        /// Is this UOW disposed?
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Saves all changes until now in this unit of work.
        /// This method may be called to apply changes whenever needed.
        /// Note that if this unit of work is transactional, saved changes are also rolled back if transaction is rolled back.
        /// No explicit call is needed to SaveChanges generally, 
        /// since all changes saved at end of a unit of work automatically.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves all changes until now in this unit of work.
        /// This method may be called to apply changes whenever needed.
        /// Note that if this unit of work is transactional, saved changes are also rolled back if transaction is rolled back.
        /// No explicit call is needed to SaveChanges generally, 
        /// since all changes saved at end of a unit of work automatically.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Disables a data filter.
        /// Does nothing if it's already disabled. 
        /// Use this method in a using statement to re-enable filter if needed.
        /// </summary>
        /// <param name="filterName">Name of the filter. <see cref="AbpDataFilters"/> for standard filters.</param>
        /// <returns>A <see cref="IDisposable"/> handle to take back the disable effect.</returns>
        IDisposable DisableFilter(string filterName);

        /// <summary>
        /// Enables a data filter.
        /// Does nothing if it's already enabled.
        /// Use this method in a using statement to re-disable filter if needed.
        /// </summary>
        /// <param name="filterName">Name of the filter. <see cref="AbpDataFilters"/> for standard filters.</param>
        /// <returns>A <see cref="IDisposable"/> handle to take back the enable effect.</returns>
        IDisposable EnableFilter(string filterName);

        /// <summary>
        /// Checks if a filter is enabled or not.
        /// </summary>
        /// <param name="filterName">Name of the filter. <see cref="AbpDataFilters"/> for standard filters.</param>
        bool IsFilterEnabled(string filterName);
    }
}