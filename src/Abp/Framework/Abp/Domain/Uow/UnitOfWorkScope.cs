using System;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// TODO@Halil: Use upper-level scope instead of starting new when available.

    /// <summary>
    /// This class is used to create a manual unit of work scope.  
    /// </summary>
    /// <remarks>
    /// <see cref="UnitOfWorkAttribute"/> can be used to mark a method as unit of work. Then the marked method
    /// will be a unit of work scope. But, somethimes you may want to restrict unit of work scope to a part of a method, 
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

        private readonly IDisposableDependencyObjectWrapper<IUnitOfWork> _unitOfWorkWrapper;

        private bool _isCommited;

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        public UnitOfWorkScope() 
            : this(true)
        {
            
        }

        /// <summary>
        /// Create a new unit of work scope.
        /// </summary>
        public UnitOfWorkScope(bool isTransactional)
        {
            _unitOfWorkWrapper = IocHelper.ResolveAsDisposable<IUnitOfWork>();
            Current = _unitOfWorkWrapper.Object;
            try
            {
                Current.Begin(isTransactional);
            }
            catch
            {
                Current = null;
            }
        }

        /// <summary>
        /// Commits the unit of work.
        /// If not commited, it's rolled back on <see cref="Dispose"/>.
        /// </summary>
        public void Commit()
        {
            _unitOfWorkWrapper.Object.End();
            _isCommited = true;
        }
        
        public void Dispose()
        {
            if (!_isCommited)
            {
                try { _unitOfWorkWrapper.Object.Cancel(); }
                catch { }
            }

            Current = null;
            _unitOfWorkWrapper.Dispose();
        }
    }
}
