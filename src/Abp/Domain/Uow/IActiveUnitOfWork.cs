using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

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
        /// Gets data filter configurations for this unit of work.
        /// </summary>
        IReadOnlyList<DataFilterConfiguration> Filters { get; }

        /// <summary>
        /// A dictionary to use for custom operations on unitOfWork
        /// </summary>
        Dictionary<string, object> Items { get; set; }

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
        /// Disables one or more data filters.
        /// Does nothing for a filter if it's already disabled. 
        /// Use this method in a using statement to re-enable filters if needed.
        /// </summary>
        /// <param name="filterNames">One or more filter names. <see cref="AbpDataFilters"/> for standard filters.</param>
        /// <returns>A <see cref="IDisposable"/> handle to take back the disable effect.</returns>
        IDisposable DisableFilter(params string[] filterNames);

        /// <summary>
        /// Enables one or more data filters.
        /// Does nothing for a filter if it's already enabled.
        /// Use this method in a using statement to re-disable filters if needed.
        /// </summary>
        /// <param name="filterNames">One or more filter names. <see cref="AbpDataFilters"/> for standard filters.</param>
        /// <returns>A <see cref="IDisposable"/> handle to take back the enable effect.</returns>
        IDisposable EnableFilter(params string[] filterNames);

        /// <summary>
        /// Checks if a filter is enabled or not.
        /// </summary>
        /// <param name="filterName">Name of the filter. <see cref="AbpDataFilters"/> for standard filters.</param>
        bool IsFilterEnabled(string filterName);

        /// <summary>
        /// Sets (overrides) value of a filter parameter.
        /// </summary>
        /// <param name="filterName">Name of the filter</param>
        /// <param name="parameterName">Parameter's name</param>
        /// <param name="value">Value of the parameter to be set</param>
        IDisposable SetFilterParameter(string filterName, string parameterName, object value);

        /// <summary>
        /// Sets/Changes Tenant's Id for this UOW.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <returns>A disposable object to restore old TenantId value when you dispose it</returns>
        IDisposable SetTenantId(int? tenantId);

        /// <summary>
        /// Sets/Changes Tenant's Id for this UOW.
        /// </summary>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="switchMustHaveTenantEnableDisable">
        /// True to enable/disable <see cref="IMustHaveTenant"/> based on given tenantId.
        /// Enables <see cref="IMustHaveTenant"/> filter if tenantId is not null.
        /// Disables <see cref="IMustHaveTenant"/> filter if tenantId is null.
        /// This value is true for <see cref="SetTenantId(int?)"/> method.
        /// </param>
        /// <returns>A disposable object to restore old TenantId value when you dispose it</returns>
        IDisposable SetTenantId(int? tenantId, bool switchMustHaveTenantEnableDisable);

        /// <summary>
        /// Gets Tenant Id for this UOW.
        /// </summary>
        /// <returns></returns>
        int? GetTenantId();
    }
}