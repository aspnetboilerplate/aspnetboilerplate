using System;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow
{
    public class ThreadStaticUnitOfWorkScopeManager : IUnitOfWorkScopeManager, ISingletonDependency
    {
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