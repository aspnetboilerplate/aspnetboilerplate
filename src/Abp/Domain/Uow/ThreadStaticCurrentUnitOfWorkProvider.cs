using System;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// ThreadStatic implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// </summary>
    public class ThreadStaticCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ISingletonDependency
    {
        /// <inheritdoc />
        [DoNotWire]
        public IUnitOfWork Current
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        [ThreadStatic]
        private static IUnitOfWork _unitOfWork;
    }
}