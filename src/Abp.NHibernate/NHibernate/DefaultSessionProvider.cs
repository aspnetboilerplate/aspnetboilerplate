using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.NHibernate.Uow;
using NHibernate;

namespace Abp.NHibernate
{
    internal sealed class DefaultSessionProvider : ISessionProvider
    {
        public static DefaultSessionProvider Instance { get { return _instance; } }

        private static readonly DefaultSessionProvider _instance = new DefaultSessionProvider();

        private DefaultSessionProvider()
        {
        }

        public ISession GetSession()
        {
            // TODO: ammachado: Remove this dependency
            return ((NhUnitOfWork)IocManager.Instance.Resolve<UnitOfWorkScope>().Current).Session;
        }
    }
}