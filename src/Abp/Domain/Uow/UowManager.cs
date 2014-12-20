using Abp.Dependency;

namespace Abp.Domain.Uow
{
    public class UowManager : IUowManager, ISingletonDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IUnitOfWorkScopeManager _unitOfWorkScopeManager;

        public IActiveUnitOfWork Current
        {
            get { return _unitOfWorkScopeManager.Current; }
        }

        public UowManager(IIocResolver iocResolver, IUnitOfWorkScopeManager unitOfWorkScopeManager)
        {
            _iocResolver = iocResolver;
            _unitOfWorkScopeManager = unitOfWorkScopeManager;
        }

        public IUnitOfWorkCompleteHandle StartNew(bool isTransactional = true)
        {
            var uow = _unitOfWorkScopeManager.Current;
            if (uow != null)
            {
                return new UnitOfWorkCompleteHandle(uow, false);
            }

            uow = _iocResolver.Resolve<IUnitOfWork>();
            uow.Disposed += (sender, args) => { _unitOfWorkScopeManager.Current = null; };
            uow.Start(isTransactional);

            _unitOfWorkScopeManager.Current = uow;
            return new UnitOfWorkCompleteHandle(uow, true);
        }
    }
}