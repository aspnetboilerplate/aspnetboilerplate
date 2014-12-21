using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager, ISingletonDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public IActiveUnitOfWork Current
        {
            get { return _currentUnitOfWorkProvider.Current; }
        }

        public UnitOfWorkManager(IIocResolver iocResolver, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _iocResolver = iocResolver;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public IUnitOfWorkCompleteHandle StartNew(bool isTransactional = true)
        {
            if (_currentUnitOfWorkProvider.Current != null)
            {
                return new NullUnitOfWorkCompleteHandle();
            }

            var uow = _iocResolver.Resolve<IUnitOfWork>();
            uow.Disposed += (sender, args) => { _currentUnitOfWorkProvider.Current = null; };
            uow.Start(isTransactional);
            
            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}