using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp.Utils.Etc;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Base for all Unit Of Work classes.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {

        /// <inheritdoc/>
        public event EventHandler Completed;

        /// <inheritdoc/>
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <inheritdoc/>
        public event EventHandler Disposed;

        /// <inheritdoc/>
        public UnitOfWorkOptions Options { get; private set; }

        /// <summary>
        /// Gets a value indicates that this unit of work is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Is <see cref="Begin"/> method called before?
        /// </summary>
        private bool _isBeginCalledBefore;

        /// <summary>
        /// Is <see cref="Complete"/> method called before?
        /// </summary>
        private bool _isCompleteCalledBefore;

        /// <summary>
        /// Is this unit of work successfully completed.
        /// </summary>
        private bool _succeed;

        /// <summary>
        /// A reference to the exception if this unit of work failed.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// Data filter configurations for this unit of work.
        /// </summary>
        protected List<DataFilterConfiguration> Filters { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected UnitOfWorkBase(IUnitOfWorkDefaultOptions defaultOptions)
        {
            Filters = defaultOptions.Filters.ToList();
        }

        /// <inheritdoc/>
        public void Begin(UnitOfWorkOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            PreventMultipleBegin();
            Options = options; //TODO: Do not set options like that!

            SetOptions(options);

            BeginUow();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync();

        public IDisposable DisableFilter(string filterName)
        {
            var filterIndex = GetFilterIndex(filterName);
            if (!Filters[filterIndex].IsEnabled)
            {
                return NullDisposable.Instance;
            }

            ApplyDisableFilter(filterName);

            Filters[filterIndex] = new DataFilterConfiguration(filterName, false);
            
            return new DisposeAction(() => EnableFilter(filterName));
        }

        public IDisposable EnableFilter(string filterName)
        {
            var filterIndex = GetFilterIndex(filterName);
            if (Filters[filterIndex].IsEnabled)
            {
                return NullDisposable.Instance;
            }

            ApplyEnableFilter(filterName);

            Filters[filterIndex] = new DataFilterConfiguration(filterName, true);
            
            return new DisposeAction(() => DisableFilter(filterName));
        }

        public virtual bool IsFilterEnabled(string filterName)
        {
            return GetFilter(filterName).IsEnabled;
        }

        /// <inheritdoc/>
        public void Complete()
        {
            PreventMultipleComplete();
            try
            {
                CompleteUow();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            PreventMultipleComplete();
            try
            {
                await CompleteUowAsync();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                OnFailed(_exception);
            }

            DisposeUow();
            OnDisposed();
        }

        /// <summary>
        /// Should be implemented by derived classes to start UOW.
        /// </summary>
        protected abstract void BeginUow();

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract void CompleteUow();

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract Task CompleteUowAsync();

        /// <summary>
        /// Should be implemented by derived classes to dispose UOW.
        /// </summary>
        protected abstract void DisposeUow();

        /// <summary>
        /// Concrete Unit of work classes should implement this
        /// method in order to disable a filter.
        /// Should not call base method since it throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="filterName">Filter name</param>
        protected virtual void ApplyDisableFilter(string filterName)
        {
            throw new NotImplementedException("DisableFilter is not implemented for " + GetType().FullName);
        }

        /// <summary>
        /// Concrete Unit of work classes should implement this
        /// method in order to enable a filter.
        /// Should not call base method since it throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="filterName">Filter name</param>
        protected virtual void ApplyEnableFilter(string filterName)
        {
            throw new NotImplementedException("EnableFilter is not implemented for " + GetType().FullName);
        }

        /// <summary>
        /// Called to trigger <see cref="Completed"/> event.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }

        /// <summary>
        /// Called to trigger <see cref="Failed"/> event.
        /// </summary>
        /// <param name="exception">Exception that cause failure</param>
        protected virtual void OnFailed(Exception exception)
        {
            Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(exception));
        }

        /// <summary>
        /// Called to trigger <see cref="Disposed"/> event.
        /// </summary>
        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this);
        }

        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new AbpException("This unit of work has started before. Can not call Start method more than once.");
            }

            _isBeginCalledBefore = true;
        }

        private void PreventMultipleComplete()
        {
            if (_isCompleteCalledBefore)
            {
                throw new AbpException("Complete is called before!");
            }

            _isCompleteCalledBefore = true;
        }

        private void SetOptions(UnitOfWorkOptions options)
        {
            for (var i = 0; i < Filters.Count; i++)
            {
                var filterOverride = options.FilterOverrides.FirstOrDefault(f => f.FilterName == Filters[i].FilterName);
                if (filterOverride != null)
                {
                    Filters[i] = filterOverride;
                }
            }
        }

        private DataFilterConfiguration GetFilter(string filterName)
        {
            var filter = Filters.FirstOrDefault(f => f.FilterName == filterName);
            if (filter == null)
            {
                throw new AbpException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filter;
        }

        private int GetFilterIndex(string filterName)
        {
            var filterIndex = Filters.FindIndex(f => f.FilterName == filterName);
            if (filterIndex < 0)
            {
                throw new AbpException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filterIndex;
        }
    }
}