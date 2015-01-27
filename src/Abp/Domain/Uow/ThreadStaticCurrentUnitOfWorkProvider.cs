using System;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// ThreadStatic implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// This is default implementation.
    /// </summary>
    public class ThreadStaticCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ISingletonDependency
    {
        /// <inheritdoc />
        [DoNotWire]
        public IUnitOfWork Current
        {
            get
            {
                if (_unitOfWork.IsDisposed)
                {
                    _unitOfWork = null;
                }
                
                return _unitOfWork;
            }

            set
            {
                _unitOfWork = value;
            }
        }

        [ThreadStatic]
        private static IUnitOfWork _unitOfWork;
    }
}