using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// TODO: Did not tested yet, do not use!
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
            Current.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _unitOfWorkWrapper.Object.Commit();
            }
            finally
            {
                Current = null;
            }
        }

        public void Dispose()
        {
            if (!_isCommited)
            {
                try { _unitOfWorkWrapper.Object.Rollback(); }
                catch { }
            }

            _unitOfWorkWrapper.Dispose();
        }
    }
}
