using System;
using System.Threading.Tasks;
using Abp.Extensions;

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
        public event EventHandler Failed;

        /// <inheritdoc/>
        public event EventHandler Disposed;

        /// <inheritdoc/>
        public UnitOfWorkOptions Options { get; private set; }

        /// <summary>
        /// Gets a value indicates that this unit of work is disposed or not.
        /// </summary>
        protected bool IsDisposed { get; private set; }
        private bool _isStarted;
        private bool _isCompleted;

        /// <inheritdoc/>
        public void Begin(UnitOfWorkOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            PreventMultipleStart();
            Options = options;
            BeginUow();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync();

        /// <inheritdoc/>
        public void Complete()
        {
            PreventMultipleComplete();

            CompleteUow();
            Completed.InvokeSafely(this);
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            PreventMultipleComplete();

            await CompleteUowAsync();
            Completed.InvokeSafely(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            DisposeUow();

            Disposed.InvokeSafely(this);
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

        private void PreventMultipleStart()
        {
            if (_isStarted)
            {
                throw new AbpException("This unit of work has started before. Can not call Start method more than once.");
            }

            _isStarted = true;
        }

        private void PreventMultipleComplete()
        {
            if (_isCompleted)
            {
                throw new AbpException("Complete is called before!");
            }

            _isCompleted = true;
        }
    }
}