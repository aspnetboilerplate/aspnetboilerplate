using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Base for UnitOfWork classes.
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

        /// <inheritdoc/>
        public void Start(bool isTransactional = true)
        {
            //TODO: Prevent multiple call

            IsTransactional = isTransactional;
            StartInternal();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        public abstract Task SaveChangesAsync();

        /// <inheritdoc/>
        public void Complete()
        {
            //TODO: Prevent multiple call

            CompleteInternal();
            Completed.InvokeSafely(this);
        }

        public async Task CompleteAsync()
        {
            //TODO: Prevent multiple call

            await CompleteInternalAsync();
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

            DisposeInternal();

            Disposed.InvokeSafely(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void StartInternal();

        protected abstract void CompleteInternal();
        
        protected abstract Task CompleteInternalAsync();

        protected abstract void DisposeInternal();
    }
}