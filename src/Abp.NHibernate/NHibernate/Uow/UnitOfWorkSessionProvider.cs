using Adorable.Dependency;
using Adorable.Domain.Uow;
using NHibernate;

namespace Adorable.NHibernate.Uow
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