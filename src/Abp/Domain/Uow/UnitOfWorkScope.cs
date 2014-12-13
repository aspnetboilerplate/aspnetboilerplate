using System;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This class is used to create a manual unit of work scope.  
    /// </summary>
    /// <remarks>
    /// <see cref="UnitOfWorkAttribute"/> can be used to mark a method as unit of work. Then the marked method
    /// will be a unit of work scope. But, sometimes you may want to restrict unit of work scope to a part of a method, 
    /// then use this class.
    /// </remarks>
    public class UnitOfWorkScope : IDisposable
    {
        private IUnitOfWork _currentUow;

        /// <summary>
        /// Unit of work object wrapper.
        /// </summary>
        private readonly IDisposableDependencyObjectWrapper<IUnitOfWork> _unitOfWorkWrapper;

        /// <summary>
        /// Is unit of work commited?
        /// </summary>
        private bool _isCommited;

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        public UnitOfWorkScope()
            : this(true, IocManager.Instance)
        {
        }

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        internal UnitOfWorkScope(IIocResolver iocResolver)
            : this(true, iocResolver)
        {
        }

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        public UnitOfWorkScope(bool isTransactional)
            : this(isTransactional, IocManager.Instance)
        {
        }

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        internal UnitOfWorkScope(bool isTransactional, IIocResolver iocResolver)
        {
            _unitOfWorkWrapper = iocResolver.ResolveAsDisposable<IUnitOfWork>();
            _currentUow = _unitOfWorkWrapper.Object;

            try
            {
                _currentUow.Initialize(isTransactional);
                _currentUow.Begin();
            }
            catch
            {
                _currentUow.Dispose();
                _currentUow = null;
                _unitOfWorkWrapper.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Commits the unit of work.
        /// If not commited, it's automatically rolled back on <see cref="Dispose"/>.
        /// </summary>
        public void Commit()
        {
            _currentUow.End();
            _isCommited = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_isCommited)
            {
                try
                {
                    _currentUow.Cancel();
                }
                catch
                {
                    // Hide errors
                }
            }

            // TODO: Should call dispose here?
            _currentUow.Dispose();
            _currentUow = null;
            _unitOfWorkWrapper.Dispose();
        }

        /// <summary>
        /// Gets current <see cref="IUnitOfWork"/> instance.
        /// It gets the right instance that is related to current scope (thread or web request).
        /// </summary>
        public IUnitOfWork Current
        {
            get { return _currentUow; }
        }
    }
}
