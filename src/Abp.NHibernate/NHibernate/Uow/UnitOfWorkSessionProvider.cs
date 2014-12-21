using Abp.Dependency;
using Abp.Domain.Uow;
using NHibernate;

namespace Abp.NHibernate.Uow
{
    public class UnitOfWorkSessionProvider : ISessionProvider, ITransientDependency
    {
        public ISession Session
        {
            get { return _unitOfWorkProvider.Current.GetSession(); }
        }
        
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;

        public UnitOfWorkSessionProvider(ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
        }
    }
}