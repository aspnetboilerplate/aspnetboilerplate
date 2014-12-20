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
        public event EventHandler Disposed;

        /// <inheritdoc/>
        public event EventHandler Completed;

        /// <inheritdoc/>
        public event EventHandler Failed;

        /// <inheritdoc/>
        public bool IsTransactional { get; private set; }
        
        /// <summary>
        /// Is this object disposed?
        /// Used to prevent multiple dispose.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        private bool _isStarted;
        private bool _isCompleted;

        /// <inheritdoc/>
        public void Start(bool isTransactional = true)
        {
            if (_isStarted)
            {
                throw new AbpException("This unit of work has started before.");
            }

            _isStarted = true;

            IsTransactional = isTransactional;
            StartUow();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync();

        /// <inheritdoc/>
        public void Complete()
        {
            SetAsCompleted();

            CompleteUow();
            Completed.InvokeSafely(this);
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            SetAsCompleted();

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
        protected abstract void StartUow();

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

        private void SetAsCompleted()
        {
            if (_isCompleted)
            {
                throw new AbpException("Complete is called before!");
            }

            _isCompleted = true;
        }
    }
}