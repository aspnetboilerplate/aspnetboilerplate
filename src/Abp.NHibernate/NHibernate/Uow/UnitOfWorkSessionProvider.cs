using Abp.Dependency;
using Abp.Domain.Uow;
using NHibernate;

namespace Abp.NHibernate.Uow
{
    public class UnitOfWorkSessionProvider : ISessionProvider, ITransientDependency
    {
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;

        public UnitOfWorkSessionProvider(ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public ISession Session
        {
            get { return _unitOfWorkProvider.Current.GetSession(); }
        }
    }
}