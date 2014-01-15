using System;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This class is experimental for now. (TODO: Test & make it usable)
    /// </summary>
    public class UnitOfWorkScope : IDisposable
    {
        /// <summary>
        /// Gets current instance of the NhUnitOfWork.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        public static IUnitOfWork Current
        {
            get { return _current; }
            set { _current = value; }
        }

        [ThreadStatic]
        private static IUnitOfWork _current;

        private readonly DisposableDependencyObjectWrapper<IUnitOfWork> _unitOfWorkWrapper;

        private bool _isCommited;

        public UnitOfWorkScope()
        {
            _unitOfWorkWrapper = IocHelper.ResolveAsDisposable<IUnitOfWork>();
            Current = _unitOfWorkWrapper.Object;
            try
            {
                Current.BeginTransaction();
            }
            catch
            {
                Current = null;
            }
        }

        public void Commit()
        {
            _unitOfWorkWrapper.Object.Commit();
            _isCommited = true;
        }

        public void Dispose()
        {
            if (!_isCommited)
            {
                try { _unitOfWorkWrapper.Object.Rollback(); }
                catch { }
            }

            Current = null;
            _unitOfWorkWrapper.Dispose();
        }
    }
}
