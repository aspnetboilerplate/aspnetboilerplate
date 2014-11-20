using System;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// TODO@Halil: Use upper-level scope instead of starting new when available..?

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
        /// <summary>
        /// Gets current <see cref="IUnitOfWork"/> instance.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        public static IUnitOfWork Current
        {
            get { return _currentUow; }
            set { _currentUow = value; }
        }

        [ThreadStatic]
        private static IUnitOfWork _currentUow;

        /// <summary>
        /// Unit of work object wrapper.
        /// </summary>
        private readonly IDisposableDependencyObjectWrapper<IUnitOfWork> _unitOfWorkWrapper;

        /// <summary>
        /// Is current unit of work started by this scope?
        /// </summary>
        private readonly bool _isStartedByThisScope;

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
            //There is already a uow, do nothing
            if (Current != null)
            {
                return;
            }

            //this scope started the uow
            _isStartedByThisScope = true;

            _unitOfWorkWrapper = iocResolver.ResolveAsDisposable<IUnitOfWork>();

            try
            {
                Current = _unitOfWorkWrapper.Object;
                Current.Initialize(isTransactional);
                Current.Begin();
            }
            catch
            {
                Current = null;
                throw;
            }
        }

        /// <summary>
        /// Commits the unit of work.
        /// If not commited, it's automatically rolled back on <see cref="Dispose"/>.
        /// </summary>
        public void Commit()
        {
            if (!_isStartedByThisScope)
            {
                //if this scope did not started the uow, do nothing
                return;
            }

            Current.End();
            _isCommited = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_isStartedByThisScope)
            {
                //if this scope did not started the uow, do nothing
                return;
            }

            if (Current == null)
            {
                //No active UOW
                return;
            }

            if (!_isCommited)
            {
                try { Current.Cancel(); }
                catch { } //Hide errors
            }

            Current = null;
            _unitOfWorkWrapper.Dispose();
        }
    }
}
