using System;
using NHibernate;

namespace Abp.NHibernate
{
    internal class FactorySessionProvider : ISessionProvider
    {
        private readonly Func<ISession> _sessionFactory;

        public FactorySessionProvider(Func<ISession> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession GetSession()
        {
            return _sessionFactory();
        }
    }
}